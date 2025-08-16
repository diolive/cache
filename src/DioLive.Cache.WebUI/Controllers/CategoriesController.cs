using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers;

[Authorize]
public class CategoriesController(
    ICurrentContext currentContext,
    ICategoriesLogic categoriesLogic,
    IPermissionsValidator permissionsValidator
) : BaseController(currentContext)
{
    public IActionResult Index()
    {
        if (!CurrentContext.BudgetId.HasValue)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Result<IReadOnlyCollection<Category>> getCategoriesResult = categoriesLogic.GetAll();

        return ProcessResult(
            getCategoriesResult,
            categories =>
            {
                Hierarchy<Category, int> hierarchy = Hierarchy.Create(
                    categories,
                    c => c.Id,
                    c => c.ParentId
                );

                Category[] orderedCategories = [.. hierarchy.Select(c => c.Value)];

                CategoryWithDepthVM[] model =
                [
                    ..hierarchy
                        .Select(node => new CategoryWithDepthVM(
                            node,
                            orderedCategories.Except(node.Values())
                        ))
                ];

                return View(model);
            });
    }

    public async Task<IActionResult> Create()
    {
        Guid? budgetId = CurrentContext.BudgetId;

        if (!budgetId.HasValue)
        {
            return RedirectToAction(
                nameof(HomeController.Index),
                "Home"
            );
        }

        Result result = await permissionsValidator.CheckUserCanCreateCategoryAsync(
            budgetId.Value,
            CurrentContext.GetUserId()
        );

        return ProcessResult(
            result,
            View
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateCategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Guid? budgetId = CurrentContext.BudgetId;

        if (!budgetId.HasValue)
        {
            return RedirectToAction(
                nameof(HomeController.Index),
                "Home"
            );
        }

        Result result = categoriesLogic.Create(model.Name);

        return ProcessResult(
            result,
            () => RedirectToAction(nameof(Index))
        );
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        int categoryId = id.Value;

        Result canDeleteResult = await permissionsValidator.CheckUserRightsForCategoryAsync(
            categoryId,
            CurrentContext.GetUserId(),
            ShareAccess.Categories
        );

        Result<Category> getCategoryResult = canDeleteResult.Then(() => categoriesLogic.Get(categoryId));

        return ProcessResult(getCategoryResult, View);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        Result result = categoriesLogic.Delete(id);

        return ProcessResult(
            result,
            () => RedirectToAction(nameof(Index))
        );
    }

    public IActionResult Latest(string purchase)
    {
        Result<int> result = categoriesLogic.GetPrevious(purchase);

        return ProcessResult(
            result,
            () => Ok(result.Value)
        );
    }
}