using System.Collections.Immutable;
using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectStatsController : Controller
{
    private readonly ILogger<ProjectStatsController> _logger;

    private readonly IProjectStatisticRepository _projectStatisticRepository;

    private readonly ICountryStatisticRepository _countryStatistic;

    public ProjectStatsController(ILogger<ProjectStatsController> logger, IProjectStatisticRepository projectStatisticRepository, ICountryStatisticRepository countryStatistic)
    {
        _logger = logger;
        _projectStatisticRepository = projectStatisticRepository;
        _countryStatistic = countryStatistic;
    }
    
    [Route("projects")]
    public async Task<IActionResult> Index()
    {
        var viewCollection = new List<ProjectsSimpleViewModel>();
    
        var collection = await _projectStatisticRepository.List();

        foreach (var project in collection)
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

            viewCollection.Add(new ProjectsSimpleViewModel
            {
                ProjectName = project.ProjectName,
                ProjectStatsUrl = project.ProjectStatisticUrl,
                TotalCredit = project.TotalCredit,
                Category = project.ProjectCategory,
                HasMoreThanZeroCreditDay = isAllCreditDayZero
            });
        }

        return View(viewCollection);
    }
}