using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectWeightController : Controller
{
private readonly ILogger<ProjectWeightController> _logger;
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
        { "einstein", (22500, "GPU") },
        { "total without asic", (22500, "GPU") },
        { "moo! wrapper", (45000, "GPU") },
        { "primegrid", (25000, "GPU") }
    };


    public ProjectWeightController(ILogger<ProjectWeightController> logger, IBoincProjectStatsRepo projectStatsRepo)
    {
        _logger = logger;
        _projectStatsRepo = projectStatsRepo;
    }

    [Route("calculation")]
    public async Task<IActionResult> Index()
    {
        var projectOverviewList = new List<ProjectWeightViewModel>();
        var projectList = await _projectStatsRepo.ListAll();

        foreach (var project in projectList)
        {
            var firstCountry = project.DetailedStatistics.FirstOrDefault(x => x.Rank == "1");
            var ukraineStats = project.DetailedStatistics.FirstOrDefault(x => x.CountryName == "Ukraine");
            var russiaStats = project.DetailedStatistics.FirstOrDefault(x => x.CountryName == "Russian Federation");

            if (ukraineStats == null || russiaStats == null || firstCountry == null)
            {
                continue;
            }

            var uaCredit = decimal.Parse(ukraineStats.TotalCredit.Replace(",", ""));
            var ruCredit = decimal.Parse(russiaStats.TotalCredit.Replace(",", ""));
            var totalCredit = decimal.Parse(project.TotalCredit.Replace(",", ""));

            var uaAverage = decimal.Parse(ukraineStats.CreditAvarage.Replace(",", ""));
            var ruAverage = decimal.Parse(russiaStats.CreditAvarage.Replace(",", ""));

            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var ruWeight = Math.Round((ruCredit / totalCredit) * 100, 2);

            var creditDifference = ruCredit - uaCredit;
            if (creditDifference < 0)
            {
                creditDifference /= ruAverage;
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


            var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : 0;

            var devicesToOvercome = Math.Round((ruAverage - uaAverage) / (creditsPerHour * 24), 0) + 1;

            projectOverviewList.Add(new ProjectWeightViewModel {
                ProjectName = project.ProjectName,
                UaWeight = (double)uaWeight,
                RuWeight = (double)ruWeight,
                CreditDifference = (double)creditDifference,
                CreditUA = ukraineStats.TotalCredit,
                CreditRU = russiaStats.TotalCredit,
                AvarageRU = russiaStats.CreditAvarage,
                AvarageUA = ukraineStats.CreditAvarage,
                TaskHours = (double)taskHours,
                YearsDifference = (double)yearsDifference,
                MWtPerHourCpu = (double)mwth,
                DevicesToOvercome = (double)devicesToOvercome,
                DaysToWin = (double)daysToWin,
                ProjectType = projectType
            });
        }

        return View(projectOverviewList);
    }
}