using DioRed.Cache.Infrastructure.Auth;
using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Domain.Repositories;
using DioRed.Cache.WebApp.Models.BudgetSharingViewModels;
using DioRed.Cache.WebApp.Models.BudgetViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioRed.Cache.WebApp.Controllers;

[Authorize]
public class BudgetsController(
    ICurrentContext currentContext,
    IBudgetsLogic budgetsLogic,
    ICurrenciesLogic currenciesLogic,
    AppUserManager userManager,
    IPermissionsValidator permissionsValidator
) : BaseController(currentContext)
{
    public IActionResult Choose(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        Guid budgetId = id.Value;
        Result<BudgetSlim> result = budgetsLogic.Open(budgetId);

        if (result.IsSuccess)
        {
            CurrentContext.SetBudget(result.Value);
        }

        return ProcessResult(result, () => RedirectToAction(nameof(PurchasesController.Index), "Purchases"));
    }

    public IActionResult Create()
    {
        FillCurrenciesList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateBudgetVM model)
    {
        if (!ModelState.IsValid)
        {
            FillCurrenciesList();
            return View(model);
        }

        Result<Guid> result = budgetsLogic.Create(
            model.Name,
            model.Currency
        );

        return ProcessResult(
            result,
            budgetId => RedirectToAction(
                nameof(Choose),
                new
                {
                    Id = budgetId
                }
            )
        );
    }

    public async Task<IActionResult> Manage()
    {
        Guid? budgetId = CurrentContext.BudgetId;

        if (!budgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Result canRenameResult = await permissionsValidator.CheckUserCanRenameBudgetAsync(
            budgetId.Value,
            CurrentContext.GetUserId()
        );

        Result<string> getNameResult = canRenameResult.Then(budgetsLogic.GetName);

        return ProcessResult(
            getNameResult,
            name => View(
                new ManageBudgetVM
                {
                    Id = budgetId.Value,
                    Name = name
                }
            )
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Manage(ManageBudgetVM model)
    {
        if (!ModelState.IsValid ||
            model.Id != CurrentContext.BudgetId)
        {
            return View(model);
        }

        Result renameResult = budgetsLogic.Rename(model.Name);

        return ProcessResult(
            renameResult,
            () => RedirectToAction(
                nameof(HomeController.Index),
                "Home"
            )
        );
    }

    [HttpPost]
    public async Task<IActionResult> Share(ShareVM model)
    {
        IdentityUser? user = await userManager.FindByNameAsync(model.UserName);

        if (user is null)
        {
            return NotFound("User not found");
        }

        string userId = await userManager.GetUserIdAsync(user);

        Result result = budgetsLogic.Share(userId, model.Access);

        return ProcessResult(
            result,
            () => RedirectToAction(
                nameof(Manage),
                new
                {
                    id = model.BudgetId
                }
            )
        );
    }

    private void FillCurrenciesList()
    {
        Result<IReadOnlyCollection<Currency>> result = currenciesLogic.GetAll();

        if (result.IsSuccess)
        {
            ViewBag.Currency = new SelectList(
                result.Value,
                "Id",
                "Sign",
                "RUB"
            );
        }
    }
}