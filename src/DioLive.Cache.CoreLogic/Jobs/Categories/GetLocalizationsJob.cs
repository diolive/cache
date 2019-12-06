using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasAnyRights]
	public class GetLocalizationsJob : Job<ILookup<int, LocalizedName>>
	{
		private readonly IReadOnlyCollection<Category> _categories;

		public GetLocalizationsJob(IReadOnlyCollection<Category> categories)
		{
			_categories = categories;
		}

		protected override async Task<ILookup<int, LocalizedName>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			ILookup<int, LocalizedName> localizations = (await Task.WhenAll(_categories
					.Select(async c => await storageCollection.Categories.GetLocalizationsAsync(c.Id))))
				.SelectMany(x => x)
				.ToLookup(c => c.CategoryId, c => new LocalizedName(c.Culture, c.Name));

			return localizations;
		}
	}
}