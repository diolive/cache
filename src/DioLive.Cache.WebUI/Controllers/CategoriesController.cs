using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models;
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
		private const string Bind_Create = nameof(CreateCategoryVM.Name);
		private const string Bind_Update = nameof(UpdateCategoryVM.Id) + "," + nameof(UpdateCategoryVM.Translates) + "," + nameof(UpdateCategoryVM.Color) + "," + nameof(UpdateCategoryVM.ParentId);
		private readonly IBudgetsStorage _budgetsStorage;

		private readonly ICategoriesStorage _categoriesStorage;
		private readonly string[] _cultures;

		public CategoriesController(DataHelper dataHelper,
									IOptions<RequestLocalizationOptions> locOptions,
									ICategoriesStorage categoriesStorage,
									IBudgetsStorage budgetsStorage)
			: base(dataHelper)
		{
			_categoriesStorage = categoriesStorage;
			_budgetsStorage = budgetsStorage;
			_cultures = locOptions.Value.SupportedUICultures.Select(culture => culture.Name).ToArray();
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			List<Category> categories = await _categoriesStorage.GetAsync(budgetId.Value);

			List<CategoryWithDepthVM> model = categories
				.Where(c => !c.ParentId.HasValue)
				.SelectMany(c => c.GetFlatTree())
				.Select(c => new CategoryWithDepthVM(c))
				.ToList();

			return View(model);
		}

		// GET: Categories/Create
		public async Task<IActionResult> Create()
		{
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = UserId;
			Budget budget = await _budgetsStorage.GetWithSharesAsync(budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Categories))
			{
				return Forbid();
			}

			return View();
		}

		// POST: Categories/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(Bind_Create)] CreateCategoryVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = UserId;

			Budget budget = await _budgetsStorage.GetWithSharesAsync(budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Categories))
			{
				return Forbid();
			}

			var category = new Category
			{
				Name = model.Name,
				OwnerId = userId,
				BudgetId = budgetId.Value
			};

			await _categoriesStorage.AddAsync(category);
			return RedirectToAction(nameof(Index));
		}

		// POST: Categories/Update
		[HttpPost]
		public async Task<IActionResult> Update([Bind(Bind_Update)] UpdateCategoryVM model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			(string name, string culture)[] translates = model.Translates?.Select((name, index) => (name, culture: _cultures[index])).ToArray();
			Result result = await _categoriesStorage.UpdateAsync(model.Id, UserId, model.ParentId, translates, model.Color);

			return ProcessResult(result, Ok, "Error occured on category update");
		}

		// GET: Categories/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Category category) = await _categoriesStorage.GetForRemoveAsync(id.Value, UserId);

			return ProcessResult(result, () => View(category));
		}

		// POST: Categories/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Result result = await _categoriesStorage.RemoveAsync(id, UserId);

			return ProcessResult(result, () => RedirectToAction(nameof(Index)));
		}

		public async Task<IActionResult> Latest(string purchase)
		{
			int? categoryId = await _categoriesStorage.GetLatestAsync(purchase);

			if (categoryId.HasValue)
			{
				return Ok(categoryId.Value);
			}

			return NotFound();
		}
	}
}