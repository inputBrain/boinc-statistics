using System.Globalization;
using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class UAvsUsaController : Controller
{
    private readonly ILogger<UAvsUsaController> _logger;
    private readonly IBoincProjectStatsRepo _projectStatsRepo;

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


    public UAvsUsaController(ILogger<UAvsUsaController> logger, IBoincProjectStatsRepo projectStatsRepo)
    {
        _logger = logger;
        _projectStatsRepo = projectStatsRepo;
    }

    [Route("ua-vs-usa")]
    public async Task<IActionResult> Index()
    {
        var projectOverviewList = new List<ProjectWeightViewModel>();
        var projectList = await _projectStatsRepo.ListAll();

        foreach (var project in projectList)
        {
            // var firstCountry = project.DetailedStatistics.FirstOrDefault(x => x.Rank == "1");
            var ukraineStats = project.DetailedStatistics.FirstOrDefault(x => x.CountryName == "Ukraine");
            var usaStats = project.DetailedStatistics.FirstOrDefault(x => x.CountryName == "United States");

            if (ukraineStats == null || usaStats == null)
            {
                continue;
            }

            var uaCredit = decimal.Parse(ukraineStats.TotalCredit.Replace(",", ""));
            var usaCredit = decimal.Parse(usaStats.TotalCredit.Replace(",", ""));
            var totalCredit = decimal.Parse(project.TotalCredit.Replace(",", ""));

            var uaAverage = decimal.Parse(ukraineStats.CreditAvarage.Replace(",", ""));
            var usaAverage = decimal.Parse(usaStats.CreditAvarage.Replace(",", ""));

            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var usaWeight = Math.Round((usaCredit / totalCredit) * 100, 2);

            var creditDifference = usaCredit - uaCredit;
            if (creditDifference < 0)
            {
                creditDifference /= usaAverage;
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


            var daysToWin = creditDifference > 0 && uaAverage > usaAverage ? Math.Round(creditDifference / (uaAverage - usaAverage), 0) + 1 : 0;
            // var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : Math.Round(creditDifference / (uaAverage - ruAverage), 0);

            var devicesToOvercome = Math.Round((usaAverage - uaAverage) / (creditsPerHour * 24), 0) + 1;


            var taskHoursCategory = _getCategory((double)taskHours);
            var yearsCategory = _getCategory((double)yearsDifference);
            var mwthCategory = _getCategory((double)mwth);
            var creditDiff = _getCategory((double)creditDifference);
            var wonDays = _getDaysToWinCategory((double)daysToWin);
            
            projectOverviewList.Add(new ProjectWeightViewModel {
                ProjectName = project.ProjectName,
                UaWeight = (double)uaWeight,
                RuWeight = (double)usaWeight,
                CreditDifference = creditDiff,
                CreditUA = ukraineStats.TotalCredit,
                CreditRU = usaStats.TotalCredit,
                AvarageRU = usaStats.CreditAvarage,
                AvarageUA = ukraineStats.CreditAvarage,
                TaskHours = taskHoursCategory,
                YearsDifference = yearsCategory,
                MWtPerHourCpu = mwthCategory,
                DevicesToOvercome = (double)devicesToOvercome,
                DaysToWin = daysToWin.ToString(CultureInfo.InvariantCulture),
                ProjectType = projectType
            });
        }

        return View(projectOverviewList);
    }
    
    
    private string _getCategory(double taskHours)
    {
        if (taskHours >= -100 && taskHours <= 0)
            return "Overcome";
        else if (taskHours >= -365 && taskHours <= -101)
            return "Won";
        else if (taskHours >= -1000 && taskHours <= -366)
            return "Ownage";
        else if (taskHours >= -10000 && taskHours <= -1001)
            return "Destroyed";
        else if (taskHours < -10001)
            return "Annihilated";
        else
            return taskHours.ToString();
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