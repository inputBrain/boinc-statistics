using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.ViewComponents;

public class RightMenu : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("/Pages/Components/RightMenuView.cshtml");
    }
}