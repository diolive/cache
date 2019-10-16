using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasAnyRights]
	public class GetAllJob : Job<IReadOnlyCollection<Category>>
	{
		private readonly string _culture;

		public GetAllJob(string culture)
		{
			_culture = culture;
		}

		protected override async Task<IReadOnlyCollection<Category>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetAllAsync(CurrentBudget, _culture);
		}
	}
}