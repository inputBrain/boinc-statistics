using BoincStatistic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectWeightController : Controller
{
    private readonly ILogger<ProjectWeightController> _logger;
    private readonly ICalculationService _calculationService;
    
    public ProjectWeightController(ILogger<ProjectWeightController> logger, ICalculationService calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }


    [Route("ua-vs-ru")]
    public async Task<IActionResult> Index()
    {
        var collection = await _calculationService.BaseCalculationByDefaultUAvsRuAsync();
        return View(collection);
    }
}