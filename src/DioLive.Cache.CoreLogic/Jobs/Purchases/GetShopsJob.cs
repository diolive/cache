using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasAnyRights]
	public class GetShopsJob : Job<IReadOnlyCollection<string>>
	{
		protected override async Task<IReadOnlyCollection<string>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Purchases.GetShopsAsync(CurrentBudget);
		}
	}
}