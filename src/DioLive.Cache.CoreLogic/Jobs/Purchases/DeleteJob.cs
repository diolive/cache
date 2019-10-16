using System;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasRights(ShareAccess.Purchases)]
	public class DeleteJob : Job
	{
		private readonly Guid _purchaseId;

		public DeleteJob(Guid purchaseId)
		{
			_purchaseId = purchaseId;
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Purchases.RemoveAsync(_purchaseId);
		}
	}
}