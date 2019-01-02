using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public class ChartsController : BaseController
	{
		private readonly IBudgetsStorage _budgetsStorage;

		public ChartsController(CurrentContext currentContext,
								IBudgetsStorage budgetsStorage)
			: base(currentContext)
		{
			_budgetsStorage = budgetsStorage;
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

			return ProcessResult(result, () => Json(GetCategoriesTotalsForLastDays(budget, days)));
		}

		public async Task<IActionResult> SunburstData(int days = 0)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			(Result result, Budget budget) = await _budgetsStorage.OpenAsync(budgetId.Value);

			return ProcessResult(result, () =>
			{
				CategoryDisplayVM[] data = GetCategoriesTotalsForLastDays(budget, days);
				return Json(new { DisplayName = "Total", Children = data, Color = "FFF" });
			});
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

			var purchases = budget.Purchases
				.Where(p => p.Cost > 0 && p.Date >= minDate && p.Date < tomorrow)
				.ToLookup(p => new { p.Category, p.Date });

			Category[] categories = purchases.Select(p => p.Key.Category.GetRoot()).Distinct().ToArray();
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
						.Where(p => p.Key.Category.GetRoot() == category && p.Key.Date >= dateFrom &&
									p.Key.Date < dateTo)
						.SelectMany(p => p)
						.Sum(p => p.Cost);
				}
			}

			return Json(new
			{
				Columns = categories.Select(cat => new
					{
						Name = cat.GetLocalizedName(currentCulture),
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

		private CategoryDisplayVM[] GetCategoriesTotalsForLastDays(Budget budget, int days)
		{
			string currentCulture = CurrentContext.UICulture;

			return budget.Categories.Where(c => !c.ParentId.HasValue)
				.Select(c => MapToVM(c, currentCulture, p => p.Cost > 0 && (days == 0 || (DateTime.Today - p.Date.Date).TotalDays <= days)))
				.ToArray();
		}

		private static CategoryDisplayVM MapToVM(Category cat,
												 string currentCulture,
												 Func<Purchase, bool> purchaseCondition)
		{
			return new CategoryDisplayVM
			{
				DisplayName = cat.GetLocalizedName(currentCulture),
				Color = cat.Color.ToString("X6"),
				TotalCost = cat.Purchases.Where(purchaseCondition).Sum(p => p.Cost),
				Children = cat.Subcategories.Select(c => MapToVM(c, currentCulture, purchaseCondition)).ToArray()
			};
		}
	}
}