using System;
using System.Collections.Concurrent;
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

        _logger.LogInformation("Starting scrape of {Total} projects (projects×pages = {P}×{Pg})",
            total, _config.Parallelism.Projects, _config.Parallelism.Pages);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _config.Parallelism.Projects,
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
        var projectRepo = context.Db.ProjectStatisticRepository;
        var countryRepo = context.Db.CountryStatisticRepository;

        await projectRepo.SetProjectStatus(project, ScrappingStatus.InProcess);
        _logger.LogInformation("{Ctx} starting (ID {Id})", ctx, project.Id);

        await _scrapeTotalsAsync(project, projectRepo, fetcher, ctx, cancellationToken);

        var maxPages = _config.Scraping.MaxPages;
        var pageDegree = Math.Max(1, Math.Min(maxPages, _config.Parallelism.Pages));
        var allRows = new ConcurrentBag<CountryStatisticModel>();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, maxPages),
            new ParallelOptions { MaxDegreeOfParallelism = pageDegree, CancellationToken = cancellationToken },
            async (page, ct) =>
            {
                try
                {
                    var rows = await _scrapeCountryPageAsync(project, page, fetcher, ctx, ct);
                    foreach (var row in rows)
                    {
                        allRows.Add(row);
                    }
                }
                catch (BlockedAfterAllRetriesException ex)
                {
                    _logger.LogError("{Ctx} page {Page} - blocked, skipping. Last: {Reason}", ctx, page + 1, ex.LastReason);
                }
            });

        _mergeCountryStats(project, allRows, out var newCountries, out var updatedCountries);

        if (newCountries.Count > 0)
        {
            await countryRepo.CreateBulk([..newCountries]);
            _logger.LogInformation("{Ctx} added {Count} new countries", ctx, newCountries.Count);
        }

        if (updatedCountries.Count > 0)
        {
            await countryRepo.UpdateBulk([..updatedCountries]);
            _logger.LogInformation("{Ctx} updated {Count} countries", ctx, updatedCountries.Count);
        }

        await projectRepo.UpdateUpdateAt(project, DateTime.UtcNow);
        await projectRepo.SetProjectStatus(project, ScrappingStatus.Completed);
    }


    private async Task _scrapeTotalsAsync(
        ProjectStatisticModel project,
        IProjectStatisticRepository projectRepo,
        IResilientFetcher fetcher,
        string ctx,
        CancellationToken cancellationToken)
    {
        var html = await fetcher.FetchAsync(project.ProjectStatisticUrl, ctx, cancellationToken);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var table = doc.DocumentNode.SelectSingleNode("//div[@class='tablescroller']//table[@id='tblStats']");
        if (table == null)
        {
            _logger.LogWarning("{Ctx} no totals table at {Url}", ctx, project.ProjectStatisticUrl);
            return;
        }

        var rows = table.SelectNodes(".//tr");
        if (rows == null)
        {
            return;
        }

        var regex = MyRegex();
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


    private async Task<List<CountryStatisticModel>> _scrapeCountryPageAsync(
        ProjectStatisticModel project,
        int page,
        IResilientFetcher fetcher,
        string projectCtx,
        CancellationToken cancellationToken)
    {
        var pageSize = _config.Scraping.PageSize;
        var offset = page * pageSize;
        var url = $"{project.CountryStatisticUrl}/0/{offset}";
        var ctx = $"{projectCtx} page {page + 1}";

        _logger.LogInformation("{Ctx} fetching {Url}", ctx, url);

        var html = await fetcher.FetchAsync(url, ctx, cancellationToken);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var table = doc.DocumentNode.SelectSingleNode("//table[@id='tblStats']/tbody");
        if (table == null)
        {
            _logger.LogWarning("{Ctx} no detailed table", ctx);
            return new List<CountryStatisticModel>();
        }

        var trs = table.SelectNodes(".//tr");
        if (trs == null || trs.Count == 0)
        {
            _logger.LogWarning("{Ctx} no rows", ctx);
            return new List<CountryStatisticModel>();
        }

        var result = new List<CountryStatisticModel>(trs.Count);
        foreach (var tr in trs)
        {
            var columns = tr.SelectNodes(".//td");
            if (columns == null || columns.Count < 14)
            {
                continue;
            }

            result.Add(new CountryStatisticModel
            {
                ProjectId = project.Id,
                Rank = columns[3]?.InnerText.Trim() ?? "0",
                CountryName = columns[4]?.InnerText.Trim() ?? "Unknown",
                TotalCredit = columns[5]?.InnerText.Trim() ?? "0",
                CreditDay = columns[6]?.InnerText.Trim() ?? "0",
                CreditWeek = columns[7]?.InnerText.Trim() ?? "0",
                CreditMonth = columns[8]?.InnerText.Trim() ?? "0",
                CreditAvarage = columns[9]?.InnerText.Trim() ?? "0",
                CreditUser = columns[11]?.InnerText.Trim() ?? "0"
            });
        }

        return result;
    }


    private static void _mergeCountryStats(
        ProjectStatisticModel project,
        IEnumerable<CountryStatisticModel> apiRows,
        out List<CountryStatisticModel> newCountries,
        out List<CountryStatisticModel> updatedCountries)
    {
        newCountries = new List<CountryStatisticModel>();
        updatedCountries = new List<CountryStatisticModel>();

        foreach (var apiModel in apiRows)
        {
            var foundCountry = project.CountryStatistics.FirstOrDefault(
                x => x.CountryName.Equals(apiModel.CountryName, StringComparison.CurrentCultureIgnoreCase));

            if (foundCountry == null)
            {
                newCountries.Add(CountryStatisticModel.CreateModel(
                    apiModel.ProjectId,
                    apiModel.Rank,
                    apiModel.CountryName,
                    apiModel.TotalCredit,
                    apiModel.CreditDay,
                    apiModel.CreditWeek,
                    apiModel.CreditMonth,
                    apiModel.CreditAvarage,
                    apiModel.CreditUser));
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
                    apiModel.CreditUser);
                updatedCountries.Add(foundCountry);
            }
        }
    }


    [GeneratedRegex(@"^\d{1,3}(,\d{3})*")]
    private static partial Regex MyRegex();
}
