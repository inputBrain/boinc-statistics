using System.Diagnostics;
using BoincStatistic.Database.CountryStatistic;
using Microsoft.AspNetCore.Mvc;
using BoincStatistic.Models;

namespace BoincStatistic.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ICountryStatisticRepository _countryStatisticRepository;


    public HomeController(ILogger<HomeController> logger, ICountryStatisticRepository countryStatisticRepository)
    {
        _logger = logger;
        _countryStatisticRepository = countryStatisticRepository;
    }


    public async Task<IActionResult> Index()
    {
        var boincStats = await _countryStatisticRepository.GetThreeCountryAsync();
        
        var model = new BoincStatsViewModel
        {
            BoincStats = boincStats,
            PageNumber = 1,
            PageSize = 1,
            TotalRecords = 3
        };

        return View(model);
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