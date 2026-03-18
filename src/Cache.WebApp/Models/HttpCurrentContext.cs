using DioRed.Cache.Infrastructure.Auth;
using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;

using Microsoft.AspNetCore.Localization;

namespace DioRed.Cache.WebApp.Models;

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