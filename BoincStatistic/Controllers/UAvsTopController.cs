using BoincStatistic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class UAvsTopController : Controller
{
    private readonly ILogger<UAvsTopController> _logger;
    private readonly ICalculationService _calculationService;


    public UAvsTopController(ILogger<UAvsTopController> logger, ICalculationService calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }


    [Route("ua-vs-top")]
    public async Task<IActionResult> Index()
    {
        var collection = await _calculationService.BaseCalculationByDefaultUAvsRuAsync(rank:"1");
        return View(collection);
    }
}