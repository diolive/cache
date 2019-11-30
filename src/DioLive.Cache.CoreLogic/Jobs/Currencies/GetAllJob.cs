using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Currencies
{
	[Authenticated]
	public class GetAllJob : Job<IReadOnlyCollection<Currency>>
	{
		protected override async Task<IReadOnlyCollection<Currency>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Currencies.GetAllAsync();
		}
	}
}