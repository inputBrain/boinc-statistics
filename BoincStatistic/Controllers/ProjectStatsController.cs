using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectStatsController : Controller
{
    private readonly ILogger<ProjectStatsController> _logger;

    private readonly IProjectStatisticRepository _projectStatisticRepository;

    public ProjectStatsController(ILogger<ProjectStatsController> logger, IProjectStatisticRepository projectStatisticRepository)
    {
        _logger = logger;
        _projectStatisticRepository = projectStatisticRepository;
    }
    
    [Route("projects")]
    public async Task<IActionResult> Index()
    {
        var viewCollection = new List<ProjectsSimpleViewModel>();
    
        var collection = await _projectStatisticRepository.ListAll();

        
        foreach (var project in collection)
        {
            var isProjectNotWorking = false;

            var hasUkraineCreditDayZero = project.CountryStatistics.First(x => x.CountryName == "Ukraine").CreditDay == "0";
            var isSinceThenZero = project.IsCreditDayZero;

            if (hasUkraineCreditDayZero && isSinceThenZero)
            {
                isProjectNotWorking = true;
            }
            
            viewCollection.Add(new ProjectsSimpleViewModel
            {
                ProjectName = project.DisplayName ?? project.ProjectName,
                ProjectStatsUrl = project.ProjectStatisticUrl,
                TotalCredit = project.TotalCredit ?? "0",
                Category = project.ProjectCategory ?? "0",
                Status = project.Status,
                Divider = project.Divider,
                ProjectType = project.Type == ProjectType.GPU ? "GPU" : "Core",
                UpdatedAt = project.UpdatedAt,
                IsCreditDayZero = isProjectNotWorking
            });
        }

        return View(viewCollection);
    }
}