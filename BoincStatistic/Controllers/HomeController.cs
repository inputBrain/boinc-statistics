using System.Diagnostics;
using BoincStatistic.Database.BoincStats;
using Microsoft.AspNetCore.Mvc;
using BoincStatistic.Models;

namespace BoincStatistic.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IBoincStatsRepository _boincStatsRepository;


    public HomeController(ILogger<HomeController> logger, IBoincStatsRepository boincStatsRepository)
    {
        _logger = logger;
        _boincStatsRepository = boincStatsRepository;
    }


    public async Task<IActionResult> Index()
    {
        var collection = await _boincStatsRepository.ListAllAsync();
        
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            }
        );
    }
}