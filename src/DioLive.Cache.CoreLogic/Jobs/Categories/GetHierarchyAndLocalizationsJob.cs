using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class GetHierarchyAndLocalizationsJob : Job<(Hierarchy<Category, int> hierarchy, ILookup<int, LocalizedName> localizations)>
	{
		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<(Hierarchy<Category, int> hierarchy, ILookup<int, LocalizedName> localizations)> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			IReadOnlyCollection<Category> categories = await storageCollection.Categories.GetAllAsync(CurrentBudget);

			Hierarchy<Category, int> hierarchy = Hierarchy.Create(categories, c => c.Id, c => c.ParentId);

			ILookup<int, LocalizedName> localizations = (await Task.WhenAll(categories
					.Select(async c => await storageCollection.Categories.GetLocalizationsAsync(c.Id))))
				.SelectMany(x => x)
				.ToLookup(c => c.CategoryId, c => new LocalizedName(c.Culture, c.Name));

			return (hierarchy, localizations);
		}
	}
}