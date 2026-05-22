using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BoincStatistic.Database;
using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Worker.Configs;
using BoincStatistic.Worker.Scraping;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoincStatistic.Worker;

public partial class BoincStatsService : BackgroundService
{
    private readonly ILogger<BoincStatsService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkerConfig _config;


    public BoincStatsService(
        ILogger<BoincStatsService> logger,
        IServiceProvider serviceProvider,
        IOptions<WorkerConfig> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = options.Value;
    }


    async protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _processScrappingAsync(stoppingToken);

            if (_config.IsDeveloperMode)
            {
                _logger.LogWarning("\n [[[ DEV MODE ]]] Scrapping done. Set IsDeveloperMode=false to run scheduled mode.\n");
                Environment.Exit(0);
            }
        }
    }


    private async Task _processScrappingAsync(CancellationToken cancellationToken)
    {
        List<ProjectStatisticModel> projects;
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgreSqlContext>();
            projects = await context.Db.ProjectStatisticRepository.ListAll();
        }

        var toProcess = projects.Skip(_config.SkipFirstN).ToList();
        var total = toProcess.Count;
        var done = 0;
        var okCount = 0;
        var errorCount = 0;

        _logger.LogInformation("Starting scrape of {Total} projects with degree of parallelism = {Degree}",
            total, _config.Parallelism.MaxDegree);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _config.Parallelism.MaxDegree,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(toProcess, options, async (project, ct) =>
        {
            var index = Interlocked.Increment(ref done);
            var ctx = $"[{project.ProjectName}] {index}/{total}";

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var fetcher = scope.ServiceProvider.GetRequiredService<IResilientFetcher>();
                var context = scope.ServiceProvider.GetRequiredService<PostgreSqlContext>();

                await _processSingleProjectAsync(project, fetcher, context, ctx, ct);

                Interlocked.Increment(ref okCount);
                _logger.LogInformation("{Ctx} - OK", ctx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (BlockedAfterAllRetriesException ex)
            {
                Interlocked.Increment(ref errorCount);
                _logger.LogError("{Ctx} - ERROR: blocked after {Attempts} attempts. Last: {Reason}",
                    ctx, ex.Attempts, ex.LastReason);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref errorCount);
                _logger.LogError(ex, "{Ctx} - ERROR: {Type}: {Message}",
                    ctx, ex.GetType().Name, ex.Message);
            }

            if (!_config.IsDeveloperMode)
            {
                var delay = Random.Shared.Next(_config.Delay.BetweenProjectsMinMs, _config.Delay.BetweenProjectsMaxMs);
                await Task.Delay(delay, ct);
            }
            else
            {
                await Task.Delay(_config.Delay.DevModeMs, ct);
            }
        });

        _logger.LogInformation("Scraping summary: OK={Ok}, ERROR={Err}, Total={Total}", okCount, errorCount, total);
    }


    private async Task _processSingleProjectAsync(
        ProjectStatisticModel project,
        IResilientFetcher fetcher,
        PostgreSqlContext context,
        string ctx,
        CancellationToken cancellationToken)
    {
        const int pageSize = 100;
        const int maxPages = 2;
        var regex = MyRegex();
        var htmlDocument = new HtmlDocument();

        var projectRepo = context.Db.ProjectStatisticRepository;
        var countryRepo = context.Db.CountryStatisticRepository;

        await projectRepo.SetProjectStatus(project, ScrappingStatus.InProcess);
        _logger.LogInformation("{Ctx} starting (ID {Id})", ctx, project.Id);

        var html = await fetcher.FetchAsync(project.ProjectStatisticUrl, ctx, cancellationToken);
        htmlDocument.LoadHtml(html);

        var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='tablescroller']//table[@id='tblStats']");
        if (table != null)
        {
            var rows = table.SelectNodes(".//tr");
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var columns = row.SelectNodes(".//td");
                    if (columns == null || columns[0]?.InnerText != "Total credit")
                    {
                        continue;
                    }

                    var totalCreditColumn = columns[1]?.InnerText.Trim() ?? "0";
                    var matchTotalCredit = regex.Match(totalCreditColumn);
                    var isCreditDayZero = totalCreditColumn.Contains("+ 0 since then");

                    _logger.LogInformation("{Ctx} total credit: {Column}", ctx, totalCreditColumn);

                    if (!ProjectStatisticModel.IsSameTotalStatsModel(project, matchTotalCredit.Value, isCreditDayZero))
                    {
                        await projectRepo.UpdateModel(project, matchTotalCredit.Value, isCreditDayZero);
                    }
                }
            }
        }
        else
        {
            _logger.LogWarning("{Ctx} no totals table at {Url}", ctx, project.ProjectStatisticUrl);
        }

        var preparedNewCountries = new List<CountryStatisticModel>();
        var preparedCountriesToUpdate = new List<CountryStatisticModel>();

        for (var page = 0; page < maxPages; page++)
        {
            var offset = page * pageSize;
            var url = $"{project.CountryStatisticUrl}/0/{offset}";
            _logger.LogInformation("{Ctx} page {Page}/{Max}: {Url}", ctx, page + 1, maxPages, url);

            string detailedHtml;
            try
            {
                detailedHtml = await fetcher.FetchAsync(url, $"{ctx} page {page + 1}", cancellationToken);
            }
            catch (BlockedAfterAllRetriesException ex)
            {
                _logger.LogError("{Ctx} page {Page} - blocked, skipping page. {Reason}", ctx, page + 1, ex.LastReason);
                continue;
            }

            htmlDocument.LoadHtml(detailedHtml);

            var detailedTable = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='tblStats']/tbody");
            if (detailedTable == null)
            {
                _logger.LogWarning("{Ctx} no detailed table at {Url}", ctx, url);
                continue;
            }

            var trs = detailedTable.SelectNodes(".//tr");
            if (trs == null || trs.Count == 0)
            {
                _logger.LogWarning("{Ctx} no rows at {Url}, stopping pagination", ctx, url);
                break;
            }

            foreach (var tr in trs)
            {
                var projectColumns = tr.SelectNodes(".//td");
                if (projectColumns == null || projectColumns.Count < 14)
                {
                    continue;
                }

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

                var foundCountry = project.CountryStatistics.FirstOrDefault(
                    x => x.CountryName.Equals(apiModel.CountryName, StringComparison.CurrentCultureIgnoreCase));

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

            if (page < maxPages - 1)
            {
                var pageDelay = _config.IsDeveloperMode
                    ? _config.Delay.DevModeMs
                    : Random.Shared.Next(_config.Delay.BetweenPagesMinMs, _config.Delay.BetweenPagesMaxMs);
                await Task.Delay(pageDelay, cancellationToken);
            }
        }

        if (preparedNewCountries.Count > 0)
        {
            await countryRepo.CreateBulk([..preparedNewCountries]);
            _logger.LogInformation("{Ctx} added {Count} new countries", ctx, preparedNewCountries.Count);
        }

        if (preparedCountriesToUpdate.Count > 0)
        {
            await countryRepo.UpdateBulk([..preparedCountriesToUpdate]);
            _logger.LogInformation("{Ctx} updated {Count} countries", ctx, preparedCountriesToUpdate.Count);
        }

        await projectRepo.UpdateUpdateAt(project, DateTime.UtcNow);
        await projectRepo.SetProjectStatus(project, ScrappingStatus.Completed);
    }


    [GeneratedRegex(@"^\d{1,3}(,\d{3})*")]
    private static partial Regex MyRegex();
}
