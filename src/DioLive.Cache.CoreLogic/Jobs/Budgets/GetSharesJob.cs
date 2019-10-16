using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasRights(ShareAccess.Manage)]
	public class GetSharesJob : Job<IReadOnlyCollection<Share>>
	{
		protected override async Task<IReadOnlyCollection<Share>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Budgets.GetSharesAsync(CurrentBudget);
		}
	}
}