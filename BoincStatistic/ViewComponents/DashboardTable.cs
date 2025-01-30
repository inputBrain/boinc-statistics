using BoincStatistic.Database.CountryStatistic;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.ViewComponents;

public class DashboardTable : ViewComponent
{
    private readonly ICountryStatisticRepository _statsRepository;


    public DashboardTable(ICountryStatisticRepository statsRepository)
    {
        _statsRepository = statsRepository;
    }

    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var collection = await _statsRepository.ListAllAsync();
        
        return View("/Pages/Components/DashboardTableView.cshtml", collection);
    }
}