using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Entities;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Charts
{
	public class GetJob : Job<ChartData>
	{
		private readonly int _days;
		private readonly int _depth;
		private readonly int _step;

		public GetJob(int days, int depth, int step)
		{
			_days = days;
			_depth = depth;
			_step = step;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<ChartData> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			string currentCulture = CurrentContext.Culture;

			int daysCount = (_days - 1) * _step + _depth;
			DateTime today = DateTime.Today;
			DateTime tomorrow = today.AddDays(1);
			DateTime minDate = tomorrow.AddDays(-daysCount);

			IReadOnlyCollection<Category> categories = await storageCollection.Categories.GetAllAsync(CurrentBudget, currentCulture);
			var allCategories = new Hierarchy<Category, int>(categories, c => c.Id, c => c.ParentId);

			ILookup<(int CategoryId, DateTime Date), Purchase> purchases = (await storageCollection.Purchases.GetForStatAsync(CurrentBudget, minDate, tomorrow))
				.ToLookup(p => (p.CategoryId, p.Date));

			Dictionary<int, Hierarchy<Category, int>.Node> roots = purchases
				.Select(p => p.Key.CategoryId)
				.Distinct()
				.ToDictionary(c => c, c => allCategories[c].Root);

			Category[] rootCategories = roots.Values.Select(r => r.Value).ToArray();
			DateTime[] dates = Enumerable.Range(0, daysCount).Select(n => minDate.AddDays(n)).ToArray();
			var statData = new int[_days][];

			for (var dy = 0; dy < statData.Length; dy++)
			{
				statData[dy] = new int[rootCategories.Length];
				DateTime dateFrom = dates[dy * _step];
				DateTime dateTo = dateFrom.AddDays(_depth);

				for (var ct = 0; ct < rootCategories.Length; ct++)
				{
					Category category = rootCategories[ct];
					statData[dy][ct] = purchases
						.Where(p => roots[p.Key.CategoryId].Value == category && p.Key.Date >= dateFrom &&
						            p.Key.Date < dateTo)
						.SelectMany(p => p)
						.Sum(p => p.Cost);
				}
			}

			var chartData = new ChartData
			{
				Columns = rootCategories.Select(cat => new ChartDataColumn
					{
						Name = cat.Name,
						Color = cat.Color.ToString("X6")
					})
					.ToArray(),
				Data = statData.Select((stat, index) => new ChartDataItem
					{
						Date = dates[index * _step].ToString(Constants.DateFormat),
						Values = stat
					})
					.ToArray()
			};
			return chartData;
		}
	}
}