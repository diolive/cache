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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class CategoriesController : BaseController
	{
		private readonly ICategoriesLogic _categoriesLogic;
		private readonly string[] _cultures;
		private readonly IPermissionsValidator _permissionsValidator;

		public CategoriesController(ICurrentContext currentContext,
		                            IOptions<RequestLocalizationOptions> locOptions,
		                            ICategoriesLogic categoriesLogic,
		                            IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_categoriesLogic = categoriesLogic;
			_permissionsValidator = permissionsValidator;
			_cultures = locOptions.Value.SupportedUICultures
				.Select(culture => culture.Name)
				.ToArray();
		}

		public IActionResult Index()
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result<IReadOnlyCollection<Category>> getCategoriesResult = _categoriesLogic.GetAll();

			Result<ILookup<int, LocalizedName>> getLocalizationsResult = getCategoriesResult.Then(_categoriesLogic.GetLocalizations);

			return ProcessResult(getLocalizationsResult, localizations =>
			{
				Hierarchy<Category, int> hierarchy = Hierarchy.Create(getCategoriesResult.Data, c => c.Id, c => c.ParentId);

				ReadOnlyCollection<Category> categories = hierarchy
					.Select(c => c.Value)
					.ToList()
					.AsReadOnly();

				ReadOnlyCollection<CategoryWithDepthVM> model = hierarchy
					.Select(node => new CategoryWithDepthVM(node, localizations[node.Value.Id].ToList().AsReadOnly(), categories.Except(node.Values())))
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

		[HttpPost]
		public IActionResult Update(UpdateCategoryVM model)
		{
			if (!ModelState.IsValid || model.Translates is null || model.Translates.Length == 0)
			{
				return BadRequest();
			}

			LocalizedName[] translates = model.Translates.Select((name, index) => new LocalizedName(_cultures[index], name)).ToArray();

			Result result = _categoriesLogic.Update(model.Id, model.ParentId, translates, model.Color);

			return ProcessResult(result, Ok);
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