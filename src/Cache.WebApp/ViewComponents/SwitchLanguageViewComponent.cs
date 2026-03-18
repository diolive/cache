using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Localization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioRed.Cache.WebApp.ViewComponents;

public class SwitchLanguageViewComponent(
    ICurrentContext currentContext
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View((currentContext.GetCulture(), Cultures.Supported));
    }
}