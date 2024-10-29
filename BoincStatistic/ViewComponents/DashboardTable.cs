using BoincStatistic.Database.BoincStats;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.ViewComponents;

public class DashboardTable : ViewComponent
{
    private readonly IBoincStatsRepository _statsRepository;


    public DashboardTable(IBoincStatsRepository statsRepository)
    {
        _statsRepository = statsRepository;
    }

    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var collection = await _statsRepository.ListAllAsync();
        
        return View("/Pages/Components/DashboardTableView.cshtml", collection);
    }
}