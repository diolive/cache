using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents;

public class UserBudgetsViewComponent(
    ICurrentContext currentContext,
    IBudgetsLogic budgetsLogic
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        string userId = currentContext.GetUserId()
            ?? throw new ApplicationException("Cannot load current user");

        Result<IReadOnlyCollection<Budget>> result = budgetsLogic.GetAllAvailable();

        if (!result.IsSuccess)
        {
            return Content(result.ErrorMessage);
        }

        ViewBag.UserId = userId;
        return View(result.Value);
    }
}