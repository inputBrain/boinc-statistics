using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.ViewComponents;

public class Header : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("/Pages/Components/HeaderView.cshtml");
    }
}