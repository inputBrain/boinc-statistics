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
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 25)
    {
        
        var totalRecords = await _projectStatisticRepository.CountAsync();
        var boincStats = await _projectStatisticRepository.GetPaginatedAsync(pageNumber, pageSize);
        
        var model = new BoincProjectStatsViewModel
        {
            ProjectStats = boincStats,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };

        return View(model);
    }

}