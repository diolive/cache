using DioLive.Cache.Common;
using DioLive.Cache.Common.Localization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI.ViewComponents;

public class SwitchLanguageViewComponent(
    ICurrentContext currentContext
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View((currentContext.GetCulture(), Cultures.Supported));
    }
}