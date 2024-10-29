using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BoincStatistic.Database;
using BoincStatistic.Database.BoincStats;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Worker;

public class BoincStatsService : BackgroundService
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
            
            var boincStatsRepository = context.Db.BoincStatsRepository;
            
            await _processScrapping(boincStatsRepository, stoppingToken);

            _logger.LogInformation("\n\t Stats scrapping completed. Start waiting");
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }



    private async Task _processScrapping(IBoincStatsRepository boincStatsRepository, CancellationToken cancellationToken)
    { 
        const string baseUrl = "https://www.boincstats.com/stats/2/country/list";
        const int pageSize = 100;
        const int maxPages = 3;

        for (var page = 0; page < maxPages; page++)
        {
            var offset = page * pageSize;
            var url = $"{baseUrl}/0/{offset}";
            Console.WriteLine($"Processing page with offset: {offset}");

            var html = await Client.GetStringAsync(url, cancellationToken);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='tblStats']/tbody");
            if (table == null)
            {
                Console.WriteLine("No table found, stopping.");
                break;
            }

            var rows = table.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
            {
                Console.WriteLine("No rows found, stopping.");
                break;
            }

            foreach (var row in rows)
            {
                var columns = row.SelectNodes(".//td");
                if (columns == null || columns.Count < 14) 
                    continue;
                
                await boincStatsRepository.CreateModel
                (
                    columns[3]?.InnerText.Trim() ?? "0",
                    columns[4]?.InnerText.Trim() ?? "0",
                    columns[5]?.InnerText.Trim() ?? "0",
                    columns[6]?.InnerText.Trim() ?? "0",
                    columns[7]?.InnerText.Trim() ?? "0",
                    columns[8]?.InnerText.Trim() ?? "0",
                    columns[9]?.InnerText.Trim() ?? "0",
                    columns[11]?.InnerText.Trim() ?? "0"
                );
                
                //
                // var stats = new BoincStatsModel
                // {
                //     Rank = columns[3]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CountryName = columns[4]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     TotalCredit = columns[5]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CreditDay = columns[6]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CreditWeek = columns[7]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CreditMonth = columns[8]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CreditAvarage = columns[9]?.InnerText.Trim().Replace(",", "") ?? "0",
                //     CreditUser = columns[11]?.InnerText.Trim().Replace(",", "") ?? "0"
                // };
                //
                // statsList.Add(stats);
            }
        }
    }
}