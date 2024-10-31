using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Database.BoincStats;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectWeightController : Controller
{
private readonly ILogger<ProjectWeightController> _logger;
    private readonly IBoincProjectStatsRepo _projectStatsRepo;
    private readonly IBoincStatsRepository _statsRepository;

    private readonly Dictionary<string, double> _creditsPerHourDictionary = new()
    {
        { "Asteroids", 45 },
        { "Climate Prediction", 70 },
        { "LODA", 50 },
        { "Milkyway", 50 },
        { "NFS", 90 },
        { "rosetta", 40 },
        { "WCD", 50 },
        { "yafu", 40 },
        { "Yoyo", 40 },
        { "Amicable Numbers", 55000 },
        { "Einstein", 22500 },
        { "Moo wraper", 45000 },
        { "PrimeGrid", 25000 }
    };

    public ProjectWeightController(ILogger<ProjectWeightController> logger, IBoincProjectStatsRepo projectStatsRepo, IBoincStatsRepository statsRepository)
    {
        _logger = logger;
        _projectStatsRepo = projectStatsRepo;
        _statsRepository = statsRepository;
    }

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
            
            var uaCredit = double.Parse(ukraineStats.TotalCredit.Replace(",", ""));
            var ruCredit = double.Parse(russiaStats.TotalCredit.Replace(",", ""));
            var totalCredit = double.Parse(project.TotalCredit.Replace(",", ""));
            
            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var ruWeight = Math.Round((ruCredit / totalCredit) * 100, 2);
            
            var creditDifference = Math.Abs(uaCredit - ruCredit);

            var creditsPerHour = _creditsPerHourDictionary.TryGetValue(project.ProjectName, out var value) ? value : 1000;

            var taskHours = Math.Round(creditDifference / creditsPerHour, 0);
            var yearsDifference = Math.Round(taskHours / 8760, 2);
            var mwthCpu = Math.Round(taskHours * 7 / 1000000, 2);
            

            // Days To Win
            var uaAverage = double.Parse(ukraineStats.CreditAvarage.Replace(",", ""));
            var ruAverage = double.Parse(russiaStats.CreditAvarage.Replace(",", ""));
            var daysToWin = creditDifference > 0 && uaAverage > ruAverage 
                ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 
                : 0;

            projectOverviewList.Add(new ProjectWeightViewModel
            {
                ProjectName = project.ProjectName,
                TotalCredit = totalCredit,
                UaWeight = uaWeight,
                RuWeight = ruWeight,
                CreditDifference = creditDifference,
                TaskHours = taskHours,
                YearsDifference = yearsDifference,
                MWtPerHourCpu = mwthCpu,
                DaysToWin = daysToWin,
                CreditUA = ukraineStats.TotalCredit,
                CreditRU = russiaStats.TotalCredit,
                AvarageRU = russiaStats.CreditAvarage,
                AvarageUA = ukraineStats.CreditAvarage
            });
        }

        return View(projectOverviewList);
    }


}