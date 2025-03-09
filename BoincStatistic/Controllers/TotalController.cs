using BoincStatistic.Models;
using BoincStatistic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class TotalController : Controller
{
    private readonly ILogger<TotalController> _logger;

    private readonly ICalculationService _calculationService;


    public TotalController(ILogger<TotalController> logger, ICalculationService calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }


    [Route("total")]
    public async Task<IActionResult> Index()
    {
        var collection = await _calculationService.BaseCalculationByDefaultUAvsRuAsync();

        var totalProject = collection.FirstOrDefault(p => p.ProjectName == "Total without ASIC");
        if (totalProject != null)
        {
            collection.Remove(totalProject);
        }
        
        var gpuProjects = collection.Where(p => p.ProjectType == "GPU").ToList();
        var cpuProjects = collection.Where(p => p.ProjectType == "Core").ToList();

        var gpuTotals = _calculationService.CalculateTotalsUaAndRuByProjectType(gpuProjects, "GPU");
        var cpuTotals = _calculationService.CalculateTotalsUaAndRuByProjectType(cpuProjects, "Core");
    
        var result = new List<TotalScoreViewModel> { gpuTotals, cpuTotals };

        return View(result);
    }
}