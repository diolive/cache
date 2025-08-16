using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models;

public class HttpCurrentContext(
    IHttpContextAccessor httpContextAccessor,
    AppUserManager userManager
) : ICurrentContext
{
    public string GetCulture()
    {
        var feature = httpContextAccessor.HttpContext!.Features.Get<IRequestCultureFeature>()!;

        return feature.RequestCulture.UICulture.Name;
    }

    public string? GetUserId()
    {
        return userManager.GetUserId(httpContextAccessor.HttpContext!.User);
    }

    public BudgetSlim? GetBudget()
    {
        if (Session.GetGuid(SessionKeys.BudgetId) is { } budgetId)
        {
            return new BudgetSlim
            {
                Id = budgetId,
                Currency = Session.GetString(SessionKeys.Currency) ?? "EUR"
            };
        }
        else
        {
            return null;
        }
    }

    public void SetBudget(BudgetSlim value)
    {
        Session.SetGuid(SessionKeys.BudgetId, value.Id);
        Session.SetString(SessionKeys.Currency, value.Currency);
    }

    public Guid? BudgetId => GetBudget()?.Id;

    private ISession Session => httpContextAccessor.HttpContext!.Session;
}