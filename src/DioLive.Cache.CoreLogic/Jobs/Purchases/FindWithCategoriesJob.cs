using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasAnyRights]
	public class FindWithCategoriesJob : Job<IReadOnlyCollection<(Purchase purchase, Category category)>>
	{
		private readonly string? _filter;

		public FindWithCategoriesJob(string? filter)
		{
			_filter = filter;
		}

		protected override async Task<IReadOnlyCollection<(Purchase purchase, Category category)>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			IReadOnlyCollection<Purchase> purchases = await storageCollection.Purchases.FindAsync(CurrentBudget, _filter);
			IReadOnlyCollection<Category> allCategories = await storageCollection.Categories.GetAllAsync(CurrentBudget);

			return purchases
				.Select(p => (purchases: p, category: allCategories.Single(c => c.Id == p.CategoryId)))
				.ToList()
				.AsReadOnly();
		}
	}
}