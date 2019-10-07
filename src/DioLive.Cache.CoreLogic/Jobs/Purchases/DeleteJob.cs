using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	public class DeleteJob : Job
	{
		private readonly Guid _purchaseId;

		public DeleteJob(Guid purchaseId)
		{
			_purchaseId = purchaseId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForPurchase(_purchaseId, ShareAccess.Purchases);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Purchases.RemoveAsync(_purchaseId);
		}
	}
}