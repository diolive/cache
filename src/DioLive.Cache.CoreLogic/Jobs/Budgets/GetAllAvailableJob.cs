using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	public class GetAllAvailableJob : Job<IReadOnlyCollection<Budget>>
	{
		protected override async Task<IReadOnlyCollection<Budget>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Budgets.GetAllAvailableAsync();
		}
	}
}