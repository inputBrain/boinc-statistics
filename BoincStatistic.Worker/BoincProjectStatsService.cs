﻿// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Text.RegularExpressions;
// using System.Threading;
// using System.Threading.Tasks;
// using BoincStatistic.Database;
// using BoincStatistic.Database.BoincProjectStats;
// using HtmlAgilityPack;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// namespace BoincStatistic.Worker;
//
// public partial class BoincProjectStatsService : BackgroundService
// {
//     private readonly ILogger<BoincProjectStatsService> _logger;
//
//     private readonly IServiceProvider _serviceProvider;
//
//     private static readonly HttpClient Client = new();
//
//
//
//     public BoincProjectStatsService(ILogger<BoincProjectStatsService> logger, IServiceProvider serviceProvider)
//     {
//         _logger = logger;
//         _serviceProvider = serviceProvider;
//     }
//
//
//     async protected override Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             using var scope = _serviceProvider.CreateScope();
//             var context = scope.ServiceProvider.GetRequiredService<PostgreSqlContext>();
//
//             var boincProjectStatsRepository = context.Db.BoincProjectStatsRepo;
//
//             await _processScrapping(boincProjectStatsRepository, stoppingToken);
//
//             _logger.LogInformation("\n\tProject Stats scrapping completed. Start waiting");
//             await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
//         }
//     }
//
//
//
//     private async Task _processScrapping(IBoincProjectStatsRepo boincProjectStatsRepo, CancellationToken cancellationToken)
//     {
//         var collection = ProjectAPIModel.ProjectListData();
//
//         var htmlDocument = new HtmlDocument();
//
//         foreach (var apiModel in collection)
//         {
//             Console.WriteLine($"Processing page with offset: {apiModel.ProjectUrl}");
//
//             var html = await Client.GetStringAsync(apiModel.ProjectUrl, cancellationToken);
//             await Task.Delay(5_000, cancellationToken);
//
//             htmlDocument.LoadHtml(html);
//
//             var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='tablescroller']//table[@id='tblStats']");
//
//
//             if (table == null)
//             {
//                 Console.WriteLine("No table found, stopping.");
//                 continue;
//             }
//
//             var rows = table.SelectNodes(".//tr");
//             if (rows == null || rows.Count == 0)
//             {
//                 Console.WriteLine("No rows found, stopping.");
//                 continue;
//             }
//
//             foreach (var row in rows)
//             {
//                 var columns = row.SelectNodes(".//td");
//                 if (columns == null || (columns[0]?.InnerText != "Total credit"))
//                     continue;
//
//                 var match = MyRegex().Match(columns[1]?.InnerText.Trim() ?? "0");
//
//                 if (match.Success)
//                 {
//                     await boincProjectStatsRepo.CreateModel(apiModel.ProjectName, apiModel.Category, match.Value);
//                 }
//                 else
//                 {
//                     Console.WriteLine("Number not found.");
//                 }
//             }
//         }
//
//
//     }
//
//
//     [GeneratedRegex(@"^\d{1,3}(,\d{3})*")]
//     private static partial Regex MyRegex();
//
//
//
//     internal class ProjectAPIModel
//     {
//         public string ProjectUrl { get; set; }
//
//         public string ProjectName { get; set; }
//
//         public string Category { get; set; }
//
//
//         public static List<ProjectAPIModel> ProjectListData()
//         {
//             var collection = new List<ProjectAPIModel>
//             {
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/-5/project/detail/",
//                     ProjectName = "Total without ASIC",
//                     Category = "Uncategorized"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/2/project/detail/",
//                     ProjectName = "Climate Prediction",
//                     Category = "Earth Sciences"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/199/project/detail/",
//                     ProjectName = "LODA",
//                     Category = "Artificial intelligence"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/61/project/detail/",
//                     ProjectName = "Milkyway",
//                     Category = "Astrophysics"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/14/project/detail/",
//                     ProjectName = "Rosetta",
//                     Category = "Biology"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/15/project/detail/",
//                     ProjectName = "World Community Grid",
//                     Category = ""
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/121/project/detail/",
//                     ProjectName = "YAFU",
//                     Category = "Mathematics"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/52/project/detail/",
//                     ProjectName = "Yoyo",
//                     Category = "Umbrella project"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/172/project/detail/",
//                     ProjectName = "Amicable Numbers",
//                     Category = "Mathematics"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/5/project/detail/",
//                     ProjectName = "Einstein",
//                     Category = "Astrophysics"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/114/project/detail/",
//                     ProjectName = "Moo! Wrapper",
//                     Category = "Mathematics"
//                 },
//                 new()
//                 {
//                     ProjectUrl = "https://www.boincstats.com/stats/11/project/detail/",
//                     ProjectName = "PrimeGrid",
//                     Category = "Mathematics"
//                 },
//             };
//
//             return collection;
//         }
//     }
// }