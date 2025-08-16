using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.ViewComponents;

public class BudgetSharingViewComponent(
    ICurrentContext currentContext,
    IBudgetsLogic budgetsLogic,
    IPermissionsValidator permissionsValidator
) : ViewComponent
{
    private static readonly SelectList _accessSelectList;

    static BudgetSharingViewComponent()
    {
        _accessSelectList = new SelectList(
            accesses,
            nameof(Access.Value),
            nameof(Access.Title)
        );
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!currentContext.BudgetId.HasValue)
        {
            throw new InvalidOperationException("No current budget");
        }

        if (currentContext.GetUserId() is not { } userId)
        {
            throw new InvalidOperationException("No user ID");
        }

        Guid budgetId = currentContext.BudgetId.Value;

        Result result = await permissionsValidator.CheckUserRightsForBudgetAsync(
            budgetId,
            userId,
            ShareAccess.Manage
        );

        if (!result.IsSuccess)
        {
            throw new ArgumentException("User doesn't have an access for sharing this budget");
        }

        ViewData["Access"] = _accessSelectList;

        Result<IReadOnlyCollection<ShareItem>> getSharesResult = budgetsLogic.GetShares();
        if (!getSharesResult.IsSuccess)
        {
            throw new ArgumentException("Cannot get budget shares: " + getSharesResult.ErrorMessage);
        }

        IReadOnlyCollection<ShareVM> shares =
        [
            .. getSharesResult.Value
                .Select(share => new ShareVM
                {
                    BudgetId = budgetId,
                    UserName = share.UserName,
                    Access = share.Access
                })
        ];

        var model = new BudgetSharingsVM { BudgetId = budgetId, Shares = shares };

        return View(model);
    }

    private static readonly Access[] accesses =
    [
        new(ShareAccess.ReadOnly, "Read only"),
        new(ShareAccess.Purchases, "Purchases"),
        new(ShareAccess.Purchases | ShareAccess.Categories, "Purchases and categories"),
        new(ShareAccess.FullAccess, "Unlimited access")
    ];

    private record Access(
        ShareAccess Value,
        string Title
    );
}