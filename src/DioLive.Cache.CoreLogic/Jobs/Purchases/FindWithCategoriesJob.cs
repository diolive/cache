using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	public class FindWithCategoriesJob : Job<IReadOnlyCollection<(Purchase purchase, Category category)>>
	{
		private readonly string? _filter;

		public FindWithCategoriesJob(string? filter)
		{
			_filter = filter;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<IReadOnlyCollection<(Purchase purchase, Category category)>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			IReadOnlyCollection<Purchase> purchases = await storageCollection.Purchases.FindAsync(CurrentBudget, _filter);
			IReadOnlyCollection<Category> allCategories = await storageCollection.Categories.GetAllAsync(CurrentBudget, CurrentContext.Culture);

			return purchases
				.Select(p => (purchases: p, category: allCategories.Single(c => c.Id == p.CategoryId)))
				.ToList()
				.AsReadOnly();
		}
	}
}