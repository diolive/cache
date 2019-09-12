using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public class ChartsController : BaseController
	{
		private readonly ICategoriesStorage _categoriesStorage;
		private readonly IPurchasesStorage _purchasesStorage;

		public ChartsController(ICurrentContext currentContext,
		                        IPurchasesStorage purchasesStorage,
		                        ICategoriesStorage categoriesStorage)
			: base(currentContext)
		{
			_purchasesStorage = purchasesStorage;
			_categoriesStorage = categoriesStorage;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> PieData(int days = 0)
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return BadRequest();
			}

			CategoryWithTotals[] data = await _categoriesStorage.GetWithTotalsAsync(CurrentContext.UICulture, days);
			return Json(data);
		}

		public async Task<IActionResult> SunburstData(int days = 0)
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return BadRequest();
			}

			CategoryWithTotals[] data = await _categoriesStorage.GetWithTotalsAsync(CurrentContext.UICulture, days);
			return Json(new { DisplayName = "Total", Children = data, Color = "FFF" });
		}

		public async Task<IActionResult> StatData(int days, int depth, int step)
		{
			if (!CurrentContext.BudgetId.HasValue)
			{
				return BadRequest();
			}

			string currentCulture = CurrentContext.UICulture;

			int daysCount = (days - 1) * step + depth;
			DateTime today = DateTime.Today;
			DateTime tomorrow = today.AddDays(1);
			DateTime minDate = tomorrow.AddDays(-daysCount);

			var allCategories = new Hierarchy<Category, int>(await _categoriesStorage.GetAllAsync(currentCulture), c => c.Id, c => c.ParentId);

			ILookup<(int CategoryId, DateTime Date), Purchase> purchases = (await _purchasesStorage.GetForStatAsync(minDate, tomorrow))
				.ToLookup(p => (p.CategoryId, p.Date));

			Dictionary<int, Hierarchy<Category, int>.Node> roots = purchases
				.Select(p => p.Key.CategoryId)
				.Distinct()
				.ToDictionary(c => c, c => allCategories[c].Root);

			Category[] categories = roots.Values.Select(r => r.Value).ToArray();
			DateTime[] dates = Enumerable.Range(0, daysCount).Select(n => minDate.AddDays(n)).ToArray();
			var statData = new int[days][];

			for (var dy = 0; dy < statData.Length; dy++)
			{
				statData[dy] = new int[categories.Length];
				DateTime dateFrom = dates[dy * step];
				DateTime dateTo = dateFrom.AddDays(depth);

				for (var ct = 0; ct < categories.Length; ct++)
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
	}
}