using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
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

		public CategoriesController(ICurrentContext currentContext,
		                            IOptions<RequestLocalizationOptions> locOptions,
		                            ICategoriesStorage categoriesStorage,
		                            IBudgetsStorage budgetsStorage)
			: base(currentContext)
		{
			_categoriesStorage = categoriesStorage;
			_budgetsStorage = budgetsStorage;
			_cultures = locOptions.Value.SupportedUICultures.Select(culture => culture.Name).ToArray();
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			IReadOnlyCollection<Category> categories = await _categoriesStorage.GetAllAsync();

			var hierarchy = new Hierarchy<Category, int>(categories, c => c.Id, c => c.ParentId);

			ReadOnlyCollection<CategoryWithDepthVM> model = (await Task.WhenAll(hierarchy
					.Select(async c => new CategoryWithDepthVM(c, await _categoriesStorage.GetLocalizationsAsync(c.Value.Id), categories.Except(c.Values())))))
				.ToList()
				.AsReadOnly();

			return View(model);
		}

		// GET: Categories/Create
		public async Task<IActionResult> Create()
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			(Result result, Budget budget) = await _budgetsStorage.GetAsync(budgetId.Value, ShareAccess.Categories);

			return ProcessResult(result, View);
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

			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result result = await _budgetsStorage.CheckAccessAsync(budgetId.Value, ShareAccess.Categories);

			IActionResult processResult = ProcessResult(result, Ok);
			if (!(processResult is OkResult))
			{
				return processResult;
			}

			await _categoriesStorage.AddAsync(model.Name);
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
			Result result = await _categoriesStorage.UpdateAsync(model.Id, model.ParentId, translates, model.Color);

			return ProcessResult(result, Ok, "Error occured on category update");
		}

		// GET: Categories/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Category category) = await _categoriesStorage.GetAsync(id.Value);

			return ProcessResult(result, () => View(category));
		}

		// POST: Categories/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Result result = await _categoriesStorage.RemoveAsync(id);

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