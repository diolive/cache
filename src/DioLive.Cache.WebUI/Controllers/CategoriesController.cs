using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class CategoriesController : BaseController
	{
		private readonly ICategoriesLogic _categoriesLogic;
		private readonly IPermissionsValidator _permissionsValidator;

		public CategoriesController(ICurrentContext currentContext,
		                            ICategoriesLogic categoriesLogic,
		                            IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_categoriesLogic = categoriesLogic;
			_permissionsValidator = permissionsValidator;
		}

		public IActionResult Index()
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result<IReadOnlyCollection<Category>> getCategoriesResult = _categoriesLogic.GetAll();

			return ProcessResult(getCategoriesResult, categories=>
			{
				Hierarchy<Category, int> hierarchy = Hierarchy.Create(categories, c => c.Id, c => c.ParentId);

				ReadOnlyCollection<Category> orderedCategories = hierarchy
					.Select(c => c.Value)
					.ToList()
					.AsReadOnly();

				ReadOnlyCollection<CategoryWithDepthVM> model = hierarchy
					.Select(node => new CategoryWithDepthVM(node, orderedCategories.Except(node.Values())))
					.ToList()
					.AsReadOnly();

				return View(model);
			});
		}

		public async Task<IActionResult> Create()
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result result = await _permissionsValidator.CheckUserCanCreateCategoryAsync(budgetId.Value, CurrentContext.UserId);

			return ProcessResult(result, View);
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
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result result = _categoriesLogic.Create(model.Name);

			return ProcessResult(result, () => RedirectToAction(nameof(Index)));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			int categoryId = id.Value;
			Result canDeleteResult = await _permissionsValidator.CheckUserRightsForCategoryAsync(categoryId, CurrentContext.UserId, ShareAccess.Categories);

			Result<Category> getCategoryResult = canDeleteResult.Then(() => _categoriesLogic.Get(categoryId));

			return ProcessResult(getCategoryResult, View);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(int id)
		{
			Result result = _categoriesLogic.Delete(id);

			return ProcessResult(result, () => RedirectToAction(nameof(Index)));
		}

		public IActionResult Latest(string purchase)
		{
			Result<int> result = _categoriesLogic.GetPrevious(purchase);

			return ProcessResult(result, () => Ok(result.Data));
		}
	}
}