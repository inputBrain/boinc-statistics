using System.Collections.Immutable;
using System.Globalization;
using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class UAvsTopController : Controller
{
    private readonly ILogger<UAvsTopController> _logger;
    private readonly IProjectStatisticRepository _projectStatisticRepository;
    private readonly ICountryStatisticRepository _countryStatistic;

    public UAvsTopController(ILogger<UAvsTopController> logger, IProjectStatisticRepository projectStatisticRepository, ICountryStatisticRepository countryStatistic)
    {
        _logger = logger;
        _projectStatisticRepository = projectStatisticRepository;
        _countryStatistic = countryStatistic;
    }
    
    private readonly Dictionary<string, (decimal CreditsPerHour, string Type)> _creditsPerHourDictionary = new()
    {
        {"asteroids", (45, "Core")},
        {"climate prediction", (70, "Core")},
        {"loda", (50, "Core")},
        {"milkyway", (50, "Core")},
        {"nfs", (90, "Core")},
        {"rosetta", (40, "Core")},
        {"world community grid", (50, "Core")},
        {"yafu", (40, "Core")},
        {"yoyo", (40, "Core")},
        {"lhc", (40, "Core")}
    };


    private readonly Dictionary<string, (decimal CreditsPerHour, string Type)> _gpuProjects = new()
    {
        { "amicable numbers", (55000, "GPU") },
        { "einstein", (20000, "GPU") },
        { "total without asic", (22500, "GPU") },
        { "moo! wrapper", (45000, "GPU") },
        { "primegrid", (7500, "GPU") },
        { "numberfields", (2000, "GPU") },
        { "gpugrid", (100000, "GPU") },
    };
    

    [Route("ua-vs-top")]
    public async Task<IActionResult> Index()
    {
        var projectOverviewList = new List<ProjectWeightViewModel>();
        var projectList = await _projectStatisticRepository.ListAll();
        var projectIds = projectList.Select(p => p.Id).ToImmutableArray();

        var allCountryStats = await _countryStatistic.ListAllCreditDayData(projectIds);
        var countryStatsByProject = allCountryStats.GroupBy(x => x.ProjectId).ToDictionary(g => g.Key, g => g.ToList());
        
        var creditDayCounts = allCountryStats
            .GroupBy(x => x.ProjectId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalCount = g.Count(),
                    CreditDayZeroCount = g.Count(x => x.CreditDay == "0")
                });

        foreach (var project in projectList)
        {
            var hasMoreThanZeroCreditDay = creditDayCounts.TryGetValue(project.Id, out var stats) && stats.TotalCount == stats.CreditDayZeroCount;
            
            if (!countryStatsByProject.TryGetValue(project.Id, out var countryStats))
            {
                continue;
            }

            var topCountryStats = countryStats.FirstOrDefault(x => x.Rank == "1");
            var ukraineStats = countryStats.FirstOrDefault(x => x.CountryName == "Ukraine");

            if (ukraineStats == null || topCountryStats == null)
            {
                continue;
            }

            var uaCredit = decimal.Parse(ukraineStats.TotalCredit.Replace(",", ""));
            var topCountryCredit = decimal.Parse(topCountryStats.TotalCredit.Replace(",", ""));
            var totalCredit = decimal.Parse(project.TotalCredit.Replace(",", ""));

            var uaAverage = decimal.Parse(ukraineStats.CreditAvarage.Replace(",", ""));
            var topCountryAverage = decimal.Parse(topCountryStats.CreditAvarage.Replace(",", ""));

            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var topCountryWeight = Math.Round((topCountryCredit / totalCredit) * 100, 2);

            var creditDifference = topCountryCredit - uaCredit;

            var foundDaysToWinWord = string.Empty;
            if (creditDifference < 0)
            {
                var copyDifference = creditDifference;
                copyDifference /= topCountryAverage;
                Math.Round(copyDifference, 2);
                
                foundDaysToWinWord = _getDaysToWinCategory((double)copyDifference);
            }

            var isGpuProject = _gpuProjects.TryGetValue(project.ProjectName.ToLower(), out var gpuInfo);
            var projectInfo = isGpuProject ? gpuInfo : (_creditsPerHourDictionary.TryGetValue(project.ProjectName.ToLower(), out var coreInfo) ? coreInfo : (22500, "Core"));
            
            var creditsPerHour = projectInfo.CreditsPerHour;
            var projectType = projectInfo.Type;
            var taskHours = Math.Round(creditDifference / creditsPerHour, 0);

            var yearsDifference = Math.Round(taskHours / 8760, 2);

            var mwthMultiplier = isGpuProject ? 150 : 7;
            
            _logger.LogDebug($"\nProject: {project.ProjectName}. MWt/h multiplier: {mwthMultiplier}");
            
            var mwth = Math.Ceiling(taskHours * mwthMultiplier / 1000000 * 100) / 100;


            var daysToWin = creditDifference > 0 && uaAverage > topCountryAverage ? Math.Round(creditDifference / (uaAverage - topCountryAverage), 0) + 1 : 0;
            // var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : Math.Round(creditDifference / (uaAverage - ruAverage), 0);

            var devicesToOvercome = Math.Round((topCountryAverage - uaAverage) / (creditsPerHour * 24), 0) + 1;
            var daysToWinAsString = daysToWin.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(foundDaysToWinWord) == false)
            {
                daysToWinAsString = foundDaysToWinWord;
            }

            projectOverviewList.Add(new ProjectWeightViewModel {
                Country = topCountryStats.CountryName,
                ProjectName = project.ProjectName,
                UaWeight = (double)uaWeight,
                RuWeight = (double)topCountryWeight,
                CreditDifference = Math.Round((double)creditDifference, 2),
                CreditUA = ukraineStats.TotalCredit,
                CreditRU = topCountryStats.TotalCredit,
                AvarageRU = topCountryStats.CreditAvarage,
                AvarageUA = ukraineStats.CreditAvarage,
                TaskHours = (double)taskHours,
                YearsDifference = (double)yearsDifference,
                MWtPerHourCpu = (double)mwth,
                DevicesToOvercome = (double)devicesToOvercome,
                DaysToWin = daysToWinAsString,
                ProjectType = projectType,
                HasMoreThanZeroCreditDay = hasMoreThanZeroCreditDay
            });
        }

        return View(projectOverviewList);
    }


    private string _getDaysToWinCategory(double daysToWin)
    {
        if (daysToWin >= -100 && daysToWin <= 0)
            return "Overcome";
        else if (daysToWin >= -365 && daysToWin <= -101)
            return "Won";
        else if (daysToWin >= -1000 && daysToWin <= -366)
            return "Ownage";
        else if (daysToWin >= -10000 && daysToWin <= -1001)
            return "Destroyed";
        else if (daysToWin < -10000)
            return "Annihilated";
        else
            return daysToWin.ToString();
    }
}