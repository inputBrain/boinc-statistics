using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.ViewComponents;

public class Footer : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("/Pages/Components/FooterView.cshtml");
    }
}