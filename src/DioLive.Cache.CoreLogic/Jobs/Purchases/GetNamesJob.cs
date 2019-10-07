using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	public class GetNamesJob : Job<IReadOnlyCollection<string>>
	{
		private readonly string _filter;

		public GetNamesJob(string filter)
		{
			_filter = filter;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<IReadOnlyCollection<string>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Purchases.GetNamesAsync(CurrentBudget, _filter);
		}
	}
}