using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class GetAllAvailableJob : Job<IReadOnlyCollection<Budget>>
	{
		protected override void Validation()
		{
			AssertUserIsAuthenticated();
		}

		protected override async Task<IReadOnlyCollection<Budget>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Budgets.GetAllAvailableAsync();
		}
	}
}