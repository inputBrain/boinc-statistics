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
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Worker;

public partial class BoincStatsService : BackgroundService
{
    private readonly ILogger<BoincStatsService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static readonly HttpClient Client = new();
    

    public BoincStatsService(ILogger<BoincStatsService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }


    async protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PostgreSqlContext>();
            
            var boincStatsRepository = context.Db.CountryStatisticRepository;
            var boincProjectStatsRepository = context.Db.ProjectStatisticRepository;
            

            // var kievTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv"));
            // var nextRunTime = kievTime.Date.AddHours(5);
            //
            // if (kievTime.Hour >= 5)
            // {
            //     nextRunTime = nextRunTime.AddDays(1);
            // }
            //
            // var delay = nextRunTime - kievTime;
            //
            // _logger.LogInformation($"\n ----- Scrapping completed. Next run time will be at: {nextRunTime:HH:mm:ss}. On: ( {nextRunTime:D} )----- \n");
            //
            // await Task.Delay(delay, stoppingToken);
            
            await _processScrapping(boincStatsRepository,boincProjectStatsRepository, stoppingToken);
        }
    }



    private async Task _processScrapping(ICountryStatisticRepository countryStatisticRepository, IProjectStatisticRepository projectStatisticRepository, CancellationToken cancellationToken)
    { 
        var random = new Random();
        var htmlDocument = new HtmlDocument();
        const int pageSize = 100;
        const int maxPages = 3;

        var collection = ProjectAPIModel.ProjectListData();
        
        foreach (var apiModel in collection)
        {
            Console.WriteLine($"Processing page with offset: {apiModel.ProjectUrl}");

            var html = await Client.GetStringAsync(apiModel.ProjectUrl, cancellationToken);

            htmlDocument.LoadHtml(html);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='tablescroller']//table[@id='tblStats']");


            if (table == null)
            {
                Console.WriteLine("No table found, stopping.");
                continue;
            }

            var rows = table.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
            {
                Console.WriteLine("No rows found, stopping.");
                continue;
            }

            foreach (var row in rows)
            {
                var columns = row.SelectNodes(".//td");
                if (columns == null || (columns[0]?.InnerText != "Total credit"))
                    continue;

                var match = MyRegex().Match(columns[1]?.InnerText.Trim() ?? "0");

                if (match.Success)
                {
                    var model = await projectStatisticRepository.GetOneByName(apiModel.ProjectName);

                    if (model != null)
                    {
                        if (ProjectStatisticModel.IsSameTotalStatsModel(model, apiModel.ProjectName, apiModel.Category, match.Value) == false)
                        {
                            await projectStatisticRepository.UpdateModel(model, apiModel.ProjectName, apiModel.Category, match.Value);
                        }
                    }
                    else
                    {
                        model = await projectStatisticRepository.CreateModel(apiModel.ProjectName, apiModel.Category, match.Value);
                    }
                    
                    for (var page = 0; page < maxPages; page++)
                    {
                        var offset = page * pageSize;
                        var url = $"{apiModel.CountryStatsUrl}/0/{offset}";
                        Console.WriteLine($"Processing page with offset: {url}");

                        var htmlDetailedPage = await Client.GetStringAsync(url, cancellationToken);
                        
                        // 7 -12 min
                        var paginationDelay = random.Next(7 * 60 * 1000, 12 * 60 * 1000);
                        _logger.LogInformation($"Paginated page = Waiting for {paginationDelay / 1000 / 60} minutes before processing the next page...");
                        // await Task.Delay(paginationDelay, cancellationToken);
                        
                        htmlDocument.LoadHtml(htmlDetailedPage);

                        var tableDetailedPage = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='tblStats']/tbody");
                        if (tableDetailedPage == null)
                        {
                            Console.WriteLine("No table found, stopping.");
                            continue;
                        }

                        var trs = tableDetailedPage.SelectNodes(".//tr");
                        if (trs == null || trs.Count == 0)
                        {
                            Console.WriteLine("No rows found, stopping.");
                            break;
                        }

                        foreach (var tr in trs)
                        {
                            var projectColumns = tr.SelectNodes(".//td");
                            if (projectColumns == null || projectColumns.Count < 14) 
                                continue;
                            
                            
                            var rank = projectColumns[3]?.InnerText.Trim() ?? "0";
                            var countryName = projectColumns[4]?.InnerText.Trim() ?? "0";
                            var totalCredit = projectColumns[5]?.InnerText.Trim() ?? "0";
                            var creditDay = projectColumns[6]?.InnerText.Trim() ?? "0";
                            var creditWeek = projectColumns[7]?.InnerText.Trim() ?? "0";
                            var creditMonth = projectColumns[8]?.InnerText.Trim() ?? "0";
                            var creditAverage = projectColumns[9]?.InnerText.Trim() ?? "0";
                            var creditUser = projectColumns[11]?.InnerText.Trim() ?? "0";

                            var tempDetailedStatisticModel = new CountryStatisticModel
                            {
                                Rank = rank,
                                CountryName = countryName,
                                TotalCredit = totalCredit,
                                CreditDay = creditDay,
                                CreditWeek = creditWeek,
                                CreditMonth = creditMonth,
                                CreditAvarage = creditAverage,
                                CreditUser = creditUser
                            };

                            var foundDetailedCountryStatistic = model.CountryStatistics.FirstOrDefault(x => x.CountryName.ToLower() == countryName.ToLower());
                            if (foundDetailedCountryStatistic == null)
                            {
                                await countryStatisticRepository.CreateModel(
                                    model.Id,
                                    rank,
                                    countryName,
                                    totalCredit,
                                    creditDay,
                                    creditWeek,
                                    creditMonth,
                                    creditAverage,
                                    creditUser
                                );
                            }

                            if (ProjectStatisticModel.IsSameDetailedStatistic(model, tempDetailedStatisticModel) == false)
                            {
                                await projectStatisticRepository.UpdateDetailedStatistics(model, tempDetailedStatisticModel);
                            }
                            
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Number not found.");
                }
            }
            
            //51 min - 1.20 h
            var delay = random.Next(51 * 60 * 1000, 80 * 60 * 1000); 
            _logger.LogInformation($"Waiting for {delay / 1000 / 60} minutes before processing the next page...");
            // await Task.Delay(delay, cancellationToken);

        }
    }

    [GeneratedRegex(@"^\d{1,3}(,\d{3})*")]
    private static partial Regex MyRegex();
    
    
    
    internal class ProjectAPIModel
    {
        public string ProjectUrl { get; set; }
        
        public string CountryStatsUrl { get; set; }

        public string ProjectName { get; set; }

        public string Category { get; set; }


        public static List<ProjectAPIModel> ProjectListData()
        {
            var collection = new List<ProjectAPIModel>
            {
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/45/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/45/country/list",
                    ProjectName = "GPUGRID",
                    Category = "Biology"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/122/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/122/country/list",
                    ProjectName = "NumberFields",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/88/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/88/country/list",
                    ProjectName = "NFS",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/-5/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/-5/country/list",
                    ProjectName = "Total without ASIC",
                    Category = "Uncategorized"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/134/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/134/country/list",
                    ProjectName = "Asteroids",
                    Category = "Astrophysics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/2/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/2/country/list",
                    ProjectName = "Climate Prediction",
                    Category = "Earth Sciences"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/199/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/199/country/list",
                    ProjectName = "LODA",
                    Category = "Artificial intelligence"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/61/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/61/country/list",
                    ProjectName = "Milkyway",
                    Category = "Astrophysics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/14/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/14/country/list",
                    ProjectName = "Rosetta",
                    Category = "Biology"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/15/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/15/country/list",
                    ProjectName = "World Community Grid",
                    Category = "Umbrella project"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/121/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/121/country/list",
                    ProjectName = "YAFU",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/52/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/52/country/list",
                    ProjectName = "Yoyo",
                    Category = "Umbrella project"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/172/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/172/country/list",
                    ProjectName = "Amicable Numbers",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/5/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/5/country/list",
                    ProjectName = "Einstein",
                    Category = "Astrophysics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/114/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/114/country/list",
                    ProjectName = "Moo! Wrapper",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/11/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/11/country/list",
                    ProjectName = "PrimeGrid",
                    Category = "Mathematics"
                },
                new()
                {
                    ProjectUrl = "https://www.boincstats.com/stats/3/project/detail/",
                    CountryStatsUrl = "https://www.boincstats.com/stats/3/country/list",
                    ProjectName = "LHC",
                    Category = "Physics"
                },
            };

            return collection;
        }
    }
}