using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	public class GetJob : Job<Purchase>
	{
		private readonly Guid _purchaseId;

		public GetJob(Guid purchaseId)
		{
			_purchaseId = purchaseId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<Purchase> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Purchases.GetAsync(_purchaseId);
		}
	}
}