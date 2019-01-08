using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public class ChartsController : BaseController
	{
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;
		private readonly IPurchasesStorage _purchasesStorage;

		public ChartsController(CurrentContext currentContext,
								IBudgetsStorage budgetsStorage,
								IPurchasesStorage purchasesStorage,
								ICategoriesStorage categoriesStorage)
			: base(currentContext)
		{
			_budgetsStorage = budgetsStorage;
			_purchasesStorage = purchasesStorage;
			_categoriesStorage = categoriesStorage;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> PieData(int days = 0)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			(Result result, Budget budget) = await _budgetsStorage.OpenAsync(budgetId.Value);

			IActionResult processResult = ProcessResult(result, Ok);
			if (!(processResult is OkResult))
			{
				return processResult;
			}

			CategoryDisplayVM[] data = await GetCategoriesTotalsForLastDays(budgetId.Value, days);
			return Json(data);
		}

		public async Task<IActionResult> SunburstData(int days = 0)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			(Result result, Budget budget) = await _budgetsStorage.OpenAsync(budgetId.Value);

			IActionResult processResult = ProcessResult(result, Ok);

			if (!(processResult is OkResult))
			{
				return processResult;
			}

			CategoryDisplayVM[] data = await GetCategoriesTotalsForLastDays(budgetId.Value, days);
			return Json(new { DisplayName = "Total", Children = data, Color = "FFF" });
		}

		public async Task<IActionResult> StatData(int days, int depth, int step)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			(Result result, Budget budget) = await _budgetsStorage.OpenAsync(budgetId.Value);

			IActionResult actionResult = ProcessResult(result, Ok);

			if (!(actionResult is OkResult))
			{
				return actionResult;
			}

			string currentCulture = CurrentContext.UICulture;

			int daysCount = (days - 1) * step + depth;
			DateTime today = DateTime.Today;
			DateTime tomorrow = today.AddDays(1);
			DateTime minDate = tomorrow.AddDays(-daysCount);

			var allCategories = new Hierarchy<Category, int>(await _categoriesStorage.GetAllAsync(budgetId.Value, currentCulture), c => c.Id, c => c.ParentId);

			var purchases = (await _purchasesStorage.FindAsync(budgetId.Value, p => p.Cost > 0 && p.Date >= minDate && p.Date < tomorrow))
				.ToLookup(p => new { p.CategoryId, p.Date });

			Dictionary<int, Hierarchy<Category, int>.Node> roots = purchases.Select(p => p.Key.CategoryId).Distinct().ToDictionary(c => c, c => allCategories[c].Root);

			Category[] categories = roots.Values.Select(r => r.Value).ToArray();
			DateTime[] dates = Enumerable.Range(0, daysCount).Select(n => minDate.AddDays(n)).ToArray();
			var statData = new int[days][];

			for (int dy = 0; dy < statData.Length; dy++)
			{
				statData[dy] = new int[categories.Length];
				DateTime dateFrom = dates[dy * step];
				DateTime dateTo = dateFrom.AddDays(depth);

				for (int ct = 0; ct < categories.Length; ct++)
				{
					Category category = categories[ct];
					statData[dy][ct] = purchases
						.Where(p => roots[p.Key.CategoryId].Value == category && p.Key.Date >= dateFrom &&
									p.Key.Date < dateTo)
						.SelectMany(p => p)
						.Sum(p => p.Cost);
				}
			}

			return Json(new
			{
				Columns = categories.Select(cat => new
					{
						cat.Name,
						Color = cat.Color.ToString("X6")
					})
					.ToArray(),
				Data = statData.Select((stat, index) => new
					{
						Date = dates[index * step].ToString(Constants.DateFormat),
						Values = stat
					})
					.ToArray()
			});
		}

		private async Task<CategoryDisplayVM[]> GetCategoriesTotalsForLastDays(Guid budgetId, int days)
		{
			string currentCulture = CurrentContext.UICulture;

			return await Task.WhenAll((await _categoriesStorage.GetAllAsync(budgetId, currentCulture)).Where(c => !c.ParentId.HasValue)
				.Select(c => MapToVM(c, p => p.Cost > 0 && (days == 0 || (DateTime.Today - p.Date.Date).TotalDays <= days)))
				.ToArray());

			async Task<CategoryDisplayVM> MapToVM(Category cat,
												  Func<Purchase, bool> purchaseCondition)
			{
				return new CategoryDisplayVM
				{
					DisplayName = cat.Name,
					Color = cat.Color.ToString("X6"),
					TotalCost = (await _purchasesStorage.FindAsync(budgetId, p => p.CategoryId == cat.Id && purchaseCondition(p))).Sum(p => p.Cost),
					Children = await Task.WhenAll((await _categoriesStorage.GetChildrenAsync(cat.Id)).Select(c => MapToVM(c, purchaseCondition)).ToArray())
				};
			}
		}
	}
}