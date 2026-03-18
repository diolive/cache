using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.ViewComponents;

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