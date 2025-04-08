using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BoincStatistic.Database;
using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Worker.Configs;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Worker;

public partial class BoincStatsService : BackgroundService
{
    private readonly ILogger<BoincStatsService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;
    
    private static readonly HttpClient Client = new();
    

    public BoincStatsService(ILogger<BoincStatsService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }


    async protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerConfig = _configuration.GetSection("WorkerConfig").Get<WorkerConfig>();

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PostgreSqlContext>();
            
            var countryStatisticRepository = context.Db.CountryStatisticRepository;
            var projectStatisticRepository = context.Db.ProjectStatisticRepository;


            if (workerConfig!.IsDeveloperMode)
            {
                await _processScrapping(countryStatisticRepository,projectStatisticRepository, stoppingToken, workerConfig!.IsDeveloperMode);
                
                _logger.LogWarning("\n [[[ DEV MODE ]]] Scrapping has been done. \n [[[ DEV MODE ]]] Please, modify appsettings.json and set IsDeveloperMode as false.\n");

                Environment.Exit(0);
            }
            
            await _processScrapping(countryStatisticRepository,projectStatisticRepository, stoppingToken);
        }
    }


    private async Task _processScrapping(
        ICountryStatisticRepository countryStatisticRepository,
        IProjectStatisticRepository projectStatisticRepository,
        CancellationToken cancellationToken,
        bool isDeveloperMode = false
    )
    {
        var random = new Random();
        var htmlDocument = new HtmlDocument();
        const int pageSize = 100;
        const int maxPages = 2;
        var regex = MyRegex();

        var preparedNewCountries = new List<CountryStatisticModel>();
        var preparedCountriesToUpdate = new List<CountryStatisticModel>();

        var collection = await projectStatisticRepository.ListAll();

        foreach (var project in collection)
        {
            try
            {
                await projectStatisticRepository.SetProjectStatus(project, ScrappingStatus.InProcess);
                _logger.LogInformation($"Processing project: ID: {project.Id} || Name:  {project.ProjectName}");

                var html = await Client.GetStringAsync(project.ProjectStatisticUrl, cancellationToken);
                htmlDocument.LoadHtml(html);

                var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='tablescroller']//table[@id='tblStats']");
                if (table == null)
                {
                    _logger.LogWarning("No table found, skipping project: {Url}", project.ProjectStatisticUrl);
                    continue;
                }

                var rows = table.SelectNodes(".//tr");
                if (rows == null || rows.Count == 0)
                {
                    _logger.LogWarning("No rows found, skipping project: {Url}", project.ProjectStatisticUrl);
                    continue;
                }

                foreach (var row in rows)
                {
                    var columns = row.SelectNodes(".//td");
                    if (columns == null || (columns[0]?.InnerText != "Total credit"))
                        continue;

                    var totalCreditColumn = columns[1]?.InnerText.Trim() ?? "0";
                    var matchTotalCredit = regex.Match(totalCreditColumn);

                    var isCreditDayZero = totalCreditColumn.Contains("+ 0 since then");

                    _logger.LogInformation("Total credit column:  {Column}", totalCreditColumn);

                    if (!ProjectStatisticModel.IsSameTotalStatsModel(project, matchTotalCredit.Value, isCreditDayZero))
                    {
                        await projectStatisticRepository.UpdateModel(project, matchTotalCredit.Value, isCreditDayZero);
                    }
                }

                for (var page = 0; page < maxPages; page++)
                {
                    var offset = page * pageSize;
                    var url = $"{project.CountryStatisticUrl}/0/{offset}";
                    _logger.LogInformation($"Processing detailed page: {url}");

                    string htmlDetailedPage;
                    try
                    {
                        htmlDetailedPage = await Client.GetStringAsync(url, cancellationToken);
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Failed to fetch detailed page: {Url}", url);
                        continue;
                    }

                    htmlDocument.LoadHtml(htmlDetailedPage);

                    var tableDetailedPage = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='tblStats']/tbody");
                    if (tableDetailedPage == null)
                    {
                        _logger.LogWarning("No detailed table found at {Url}, skipping.", url);
                        continue;
                    }

                    var trs = tableDetailedPage.SelectNodes(".//tr");
                    if (trs == null || trs.Count == 0)
                    {
                        _logger.LogWarning("No rows in detailed stats for {Url}, stopping pagination.", url);
                        break;
                    }

                    foreach (var tr in trs)
                    {
                        var projectColumns = tr.SelectNodes(".//td");
                        if (projectColumns == null || projectColumns.Count < 14)
                            continue;

                        var apiModel = new CountryStatisticModel
                        {
                            ProjectId = project.Id,
                            Rank = projectColumns[3]?.InnerText.Trim() ?? "0",
                            CountryName = projectColumns[4]?.InnerText.Trim() ?? "Unknown",
                            TotalCredit = projectColumns[5]?.InnerText.Trim() ?? "0",
                            CreditDay = projectColumns[6]?.InnerText.Trim() ?? "0",
                            CreditWeek = projectColumns[7]?.InnerText.Trim() ?? "0",
                            CreditMonth = projectColumns[8]?.InnerText.Trim() ?? "0",
                            CreditAvarage = projectColumns[9]?.InnerText.Trim() ?? "0",
                            CreditUser = projectColumns[11]?.InnerText.Trim() ?? "0"
                        };

                        var foundCountry = project.CountryStatistics.FirstOrDefault(x => x.CountryName.Equals(apiModel.CountryName, StringComparison.CurrentCultureIgnoreCase));

                        if (foundCountry == null)
                        {
                            var newCountry = CountryStatisticModel.CreateModel(
                                apiModel.ProjectId,
                                apiModel.Rank,
                                apiModel.CountryName,
                                apiModel.TotalCredit,
                                apiModel.CreditDay,
                                apiModel.CreditWeek,
                                apiModel.CreditMonth,
                                apiModel.CreditAvarage,
                                apiModel.CreditUser
                            );

                            preparedNewCountries.Add(newCountry);
                        }
                        else if (!ProjectStatisticModel.IsSameDetailedStatistic(project, apiModel))
                        {
                            foundCountry.Update(
                                foundCountry,
                                apiModel.Rank,
                                apiModel.CountryName,
                                apiModel.TotalCredit,
                                apiModel.CreditDay,
                                apiModel.CreditWeek,
                                apiModel.CreditMonth,
                                apiModel.CreditAvarage,
                                apiModel.CreditUser
                            );
                            preparedCountriesToUpdate.Add(foundCountry);
                        }
                    }
                    
                    if (isDeveloperMode == false)
                    {
                        // 7 -12 min
                        var paginationDelay = random.Next(7 * 60 * 1000, 12 * 60 * 1000);
                        _logger.LogInformation($"Paginated page = Waiting for {paginationDelay / 1000 / 60} minutes before processing the next page...");
                        await Task.Delay(paginationDelay, cancellationToken);
                    }
                    
                    if (isDeveloperMode)
                    {
                        await Task.Delay(3_000, cancellationToken);
                    }
                    
                }
                if (preparedNewCountries.Any())
                {
                    await countryStatisticRepository.CreateBulk([..preparedNewCountries]);
                    _logger.LogInformation("Added {Count} new country records.", preparedNewCountries.Count);
                    preparedNewCountries.Clear();
                }

                if (preparedCountriesToUpdate.Any())
                {
                    await countryStatisticRepository.UpdateBulk([..preparedCountriesToUpdate]);
                    _logger.LogInformation("Updated {Count} country records.", preparedCountriesToUpdate.Count);
                    preparedCountriesToUpdate.Clear();
                }

                
                await projectStatisticRepository.UpdateUpdateAt(project, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing project: {Url}", project.ProjectStatisticUrl);
            }
            
            if (isDeveloperMode == false)
            {
                //51 min - 1.20 h
                var delay = random.Next(51 * 60 * 1000, 80 * 60 * 1000); 
                _logger.LogInformation($"Waiting for {delay / 1000 / 60} minutes before processing the next project...");
                await Task.Delay(delay, cancellationToken);
            }
                    
            if (isDeveloperMode)
            {
                await Task.Delay(15_000, cancellationToken);
            }
            
            await projectStatisticRepository.SetProjectStatus(project, ScrappingStatus.Completed);
            _logger.LogInformation("\nProject {ProjectName} marked as Completed\n", project.ProjectName);

        }

        _logger.LogInformation("Scraping completed.");
    }

    [GeneratedRegex(@"^\d{1,3}(,\d{3})*")]
    private static partial Regex MyRegex();
}