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
        var collection = await _projectStatisticRepository.List();
    
        var projectIds = collection.Select(p => p.Id).ToImmutableArray();

        var allCreditData = await _countryStatistic.ListAllCreditDayData(projectIds);

        var creditDayCounts = allCreditData
            .GroupBy(x => x.ProjectId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalCount = g.Count(),
                    CreditDayZeroCount = g.Count(x => x.CreditDay == "0")
                });

        var viewCollection = collection.Select(project =>
        {
            var stats = creditDayCounts.GetValueOrDefault(project.Id);
            return new ProjectsSimpleViewModel
            {
                ProjectName = project.ProjectName,
                TotalCredit = project.TotalCredit,
                Category = project.ProjectCategory,
                HasMoreThanZeroCreditDay = stats != null && stats.TotalCount == stats.CreditDayZeroCount
            };
        }).ToList();

        return View(viewCollection);
    }


}