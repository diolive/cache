using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.Controllers;

[Authorize]
public class PurchasesController(
    ICurrentContext currentContext,
    IBudgetsLogic budgetsLogic,
    ICategoriesLogic categoriesLogic,
    IOptionsLogic optionsLogic,
    IPlansLogic plansLogic,
    IPurchasesLogic purchasesLogic,
    IPermissionsValidator permissionsValidator
) : BaseController(currentContext)
{
    public IActionResult Index(string? filter = null)
    {
        BudgetSlim? budget = CurrentContext.GetBudget();
        if (budget is null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Result<(string name, string authorName)> getBudgetResult = budgetsLogic.GetNameAndAuthor();

        Result<Options> getOptionsResult = getBudgetResult.Then(_ => optionsLogic.Get());

        Result<IReadOnlyCollection<(Purchase purchase, Category category)>> getPurchasesResult = getBudgetResult.Then(_ => purchasesLogic.FindWithCategories(filter));

        return ProcessResult(getPurchasesResult, purchases =>
        {
            IReadOnlyCollection<PlanVM>? plans = null;

            if (getOptionsResult.Value.ShowPlanList)
            {
                Result<IReadOnlyCollection<Plan>> getPlansResult = plansLogic.GetAll();

                if (!getPlansResult.IsSuccess)
                {
                    return ProcessResult(getPlansResult);
                }

                plans = [.. getPlansResult.Value.Select(PlanVM.Build)];
            }

            ViewData["BudgetId"] = budget.Id;
            ViewData["BudgetName"] = getBudgetResult.Value.name;
            ViewData["BudgetAuthor"] = getBudgetResult.Value.authorName;
            ViewData["PurchaseGrouping"] = getOptionsResult.Value.PurchaseGrouping;
            ViewData["Plans"] = plans;

            return View(purchases
                .Select(p => PurchaseVM.Build(p.purchase, p.category, budget.Currency))
                .ToList()
                .AsReadOnly());
        });
    }

    public async Task<IActionResult> Create(int? planId = null)
    {
        Guid? budgetId = CurrentContext.BudgetId;
        if (!budgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Result canCreateResult = await permissionsValidator.CheckUserRightsForBudgetAsync(budgetId.Value, CurrentContext.GetUserId(), ShareAccess.Purchases);

        string? planName = null;
        canCreateResult.Then(() =>
        {
            if (planId.HasValue)
            {
                plansLogic.GetName(planId.Value).Then(name => planName = name);
            }
        });

        return ProcessResult(canCreateResult, () =>
        {
            FillCategoryList();

            return View(new CreatePurchaseVM
            {
                Date = DateTime.Today,
                PlanId = planId,
                Name = planName ?? ""
            });
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreatePurchaseVM model, bool oneMore = false)
    {
        Guid? budgetId = CurrentContext.BudgetId;
        if (!budgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (!ModelState.IsValid)
        {
            FillCategoryList();
            return View(model);
        }

        Result result = purchasesLogic.Create(model.Name, model.CategoryId, model.Date, model.Cost ?? 0, model.Shop, model.Comments, model.PlanId);

        return ProcessResult(result, () =>
        {
            if (!oneMore)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.Clear();

            model.Comments = null;
            model.Cost = null;
            model.Name = "";
            model.PlanId = null;

            FillCategoryList();
            return View(model);
        });
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        Guid? budgetId = CurrentContext.BudgetId;
        if (!budgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (!id.HasValue)
        {
            return NotFound();
        }

        Result canEditResult = await permissionsValidator.CheckUserCanEditPurchaseAsync(id.Value, CurrentContext.GetUserId());

        Result<PurchaseWithNames> getPurchaseResult = canEditResult.Then(() => purchasesLogic.GetWithNames(id.Value));

        return ProcessResult(getPurchaseResult, purchase =>
        {
            FillCategoryList();

            return View(EditPurchaseVM.Build(purchase.Purchase, purchase.AuthorName, purchase.LastEditorName));
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, EditPurchaseVM model)
    {
        Guid? budgetId = CurrentContext.BudgetId;
        if (!budgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            FillCategoryList();
            return View(model);
        }

        Result updateResult = purchasesLogic.Update(id, model.Name, model.CategoryId, model.Date, model.Cost, model.Shop, model.Comments);

        return ProcessResult(updateResult, () => RedirectToAction(nameof(Index)));
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        BudgetSlim? budget = CurrentContext.GetBudget();
        if (budget is null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Result canDeleteResult = await permissionsValidator.CheckUserCanDeletePurchaseAsync(id.Value, CurrentContext.GetUserId());

        Result<Purchase> getResult = canDeleteResult.Then(() => purchasesLogic.Get(id.Value));

        return ProcessResult(getResult, purchase => View(DeletePurchaseVM.Build(purchase, budget.Currency)));
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        Result result = purchasesLogic.Delete(id);

        return ProcessResult(result, () => RedirectToAction(nameof(Index)));
    }

    public IActionResult Shops()
    {
        Result<IReadOnlyCollection<string>> result = purchasesLogic.GetShops();

        return ProcessResult(result, Json);
    }

    public IActionResult Names(string q)
    {
        Result<IReadOnlyCollection<string>> result = purchasesLogic.GetNames(q);

        return ProcessResult(result, Json);
    }

    [HttpPost]
    public IActionResult AddPlan(string name)
    {
        Result<Plan> result = plansLogic.Create(name);

        return ProcessResult(result, plan => Json(PlanVM.Build(plan)));
    }

    [HttpPost]
    public IActionResult RemovePlan(int id)
    {
        Result result = plansLogic.Delete(id);

        return ProcessResult(result, Ok);
    }

    private void FillCategoryList()
    {
        Result<IReadOnlyCollection<Category>> getCategoriesResult = categoriesLogic.GetAll();

        IReadOnlyCollection<Category> categories = getCategoriesResult.Value;

        var model = categories
            .Select(cat =>
            {
                return new
                {
                    cat.Id,
                    cat.Name,
                    Parent = cat.ParentId.HasValue
                        ? categories.Single(c => c.Id == cat.ParentId.Value).Name
                        : cat.Name
                };
            })
            .ToList();

        List<string> parents = model.Select(cat => cat.Parent).Distinct().ToList();

        foreach (var children in parents
            .Select(parent => (parent, children: model.Where(c => c.Parent == parent).ToList()))
            .Where(x => x.children.Count == 1 && x.parent == x.children[0].Name)
            .Select(x => x.children[0]))
        {
            model.Remove(children);
            model.Add(new
            {
                children.Id,
                children.Name,
                Parent = "�"
            });
        }

        model =
        [
            .. model
                .OrderBy(c => c.Parent)
                .ThenBy(c => c.Name)
        ];

        Result<int?> getMostPopularResult = categoriesLogic.GetMostPopularId();

        getMostPopularResult.Then(id =>
        {
            ViewData["CategoryId"] = new SelectList(model, "Id", "Name", id, "Parent");
        });
    }
}