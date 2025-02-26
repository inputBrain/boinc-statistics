using System.Globalization;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectWeightController : Controller
{
    private readonly ILogger<ProjectWeightController> _logger;
    private readonly IProjectStatisticRepository _projectStatisticRepository;
    
    public ProjectWeightController(ILogger<ProjectWeightController> logger, IProjectStatisticRepository projectStatisticRepository)
    {
        _logger = logger;
        _projectStatisticRepository = projectStatisticRepository;
    }


    [Route("ua-vs-ru")]
    public async Task<IActionResult> Index()
    {
        var projectOverviewList = new List<ProjectWeightViewModel>();

        var projectList = await _projectStatisticRepository.ListAll();

        foreach (var project in projectList)
        {
            var totalCount = 0;
            var creditDayZeroCount = 0;

            foreach (var stat in project.CountryStatistics)
            {
                totalCount++;
                if (stat.CreditDay == "0")
                {
                    creditDayZeroCount++;
                }
            }

            var isAllCreditDayZero = totalCount == creditDayZeroCount;
            
            var ukraineStats = project.CountryStatistics.FirstOrDefault(x => x.CountryName == "Ukraine");
            var russiaStats = project.CountryStatistics.FirstOrDefault(x => x.CountryName == "Russian Federation");

            if (ukraineStats == null || russiaStats == null)
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

            var foundDaysToWinWord = string.Empty;
            if (creditDifference < 0)
            {
                var copyDifference = creditDifference / ruAverage;
                foundDaysToWinWord = _getDaysToWinCategory((double)copyDifference);
            }
            
            var creditsPerHour = project.Divider;
            var projectType = project.Type;
            var taskHours = Math.Round(creditDifference / creditsPerHour, 0);

            var yearsDifference = Math.Round(taskHours / 8760, 2);

            var mwthMultiplier = project.Type == ProjectType.GPU ? 150 : 7;
            
            _logger.LogDebug($"\nProject: {project.ProjectName}. MWt/h multiplier: {mwthMultiplier}");
            
            var mwth = Math.Ceiling(taskHours * mwthMultiplier / 1000000 * 100) / 100;


            var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : 0;
            // var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : Math.Round(creditDifference / (uaAverage - ruAverage), 0);

            var devicesToOvercome = Math.Round((ruAverage - uaAverage) / (creditsPerHour * 24), 0) + 1;
            var daysToWinAsString = daysToWin.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(foundDaysToWinWord) == false)
            {
                daysToWinAsString = foundDaysToWinWord;
            }

            if (isAllCreditDayZero)
            {
                daysToWinAsString = "0";
            }

            projectOverviewList.Add(new ProjectWeightViewModel {
                ProjectName = project.ProjectName,
                ProjectStatsUrl = project.ProjectStatisticUrl,
                CountryStatsUrl = project.CountryStatisticUrl,
                UaWeight = (double)uaWeight,
                RuWeight = (double)ruWeight,
                CreditDifference = Math.Round((double)creditDifference, 2),
                CreditUA = ukraineStats.TotalCredit,
                CreditRU = russiaStats.TotalCredit,
                AvarageRU = russiaStats.CreditAvarage,
                AvarageUA = ukraineStats.CreditAvarage,
                TaskHours = (double)taskHours,
                YearsDifference = (double)yearsDifference,
                MWtPerHourCpu = (double)mwth,
                DevicesToOvercome = (double)devicesToOvercome,
                DaysToWin = daysToWinAsString,
                ProjectType = projectType == ProjectType.GPU ? "GPU" : "Core",
                HasMoreThanZeroCreditDay = isAllCreditDayZero
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