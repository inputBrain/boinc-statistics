using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectWeightController : Controller
{
private readonly ILogger<ProjectWeightController> _logger;
    private readonly IBoincProjectStatsRepo _projectStatsRepo;

    private readonly Dictionary<string, decimal> _creditsPerHourDictionary = new()
    {
        {"asteroids", 45},
        {"climate prediction", 70}, 
        {"loda", 50},
        {"milkyway", 50},
        {"nfs", 90},
        {"rosetta", 40},
        {"world community grid", 50},
        {"yafu", 40},
        {"yoyo", 40},
        {"lth", 40}
    };

    private readonly Dictionary<string, decimal> _gpuProjects = new()
    {
        { "amicable numbers", 55000 },
        { "einstein", 22500 },
        { "total without asic", 22500 },
        { "moo wraper", 45000 },
        { "primegrid", 25000 }
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

            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var ruWeight = Math.Round((ruCredit / totalCredit) * 100, 2);

            var creditDifference = Math.Abs(ruCredit - uaCredit);

            var isGpuProject = _gpuProjects.TryGetValue(project.ProjectName.ToLower(), out var creditsPerHourGpu);
            var creditsPerHour = isGpuProject ? creditsPerHourGpu : (_creditsPerHourDictionary.TryGetValue(project.ProjectName.ToLower(), out var value) ? value : 22500);

            var taskHours = Math.Round(creditDifference / creditsPerHour, 0);
            var yearsDifference = Math.Round(taskHours / 8760, 2);

            var mwthMultiplier = isGpuProject ? 150 : 7;
            var mwth = Math.Round(taskHours * mwthMultiplier / 1000000, 2);

            var uaAverage = decimal.Parse(ukraineStats.CreditAvarage.Replace(",", ""));
            var ruAverage = decimal.Parse(russiaStats.CreditAvarage.Replace(",", ""));
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
                DaysToWin = (double)daysToWin
            });
        }

        return View(projectOverviewList);
    }
}