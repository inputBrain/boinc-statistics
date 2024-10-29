using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Database.BoincStats;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectStatsController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IBoincProjectStatsRepo _boincProjectStatsRepo;


    public ProjectStatsController(ILogger<HomeController> logger, IBoincProjectStatsRepo boincProjectStatsRepo)
    {
        _logger = logger;
        _boincProjectStatsRepo = boincProjectStatsRepo;
    }
    
    
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 25)
    {
        
        var totalRecords = await _boincProjectStatsRepo.CountAsync();
        var boincStats = await _boincProjectStatsRepo.GetPaginatedAsync(pageNumber, pageSize);
        
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