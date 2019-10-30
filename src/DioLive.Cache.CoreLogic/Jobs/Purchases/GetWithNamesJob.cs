using System;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasAnyRights]
	public class GetWithNamesJob : Job<PurchaseWithNames?>
	{
		private readonly Guid _purchaseId;

		public GetWithNamesJob(Guid purchaseId)
		{
			_purchaseId = purchaseId;
		}

		protected override async Task<PurchaseWithNames?> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			Purchase? purchase = await storageCollection.Purchases.GetAsync(_purchaseId);

			if (purchase is null)
			{
				return null;
			}

			string? authorName = await storageCollection.Users.GetNameByIdAsync(purchase.AuthorId);

			string? lastEditorName = purchase.LastEditorId is null
				? null
				: await storageCollection.Users.GetNameByIdAsync(purchase.LastEditorId);

			return new PurchaseWithNames { Purchase = purchase, AuthorName = authorName, LastEditorName = lastEditorName };
		}
	}
}