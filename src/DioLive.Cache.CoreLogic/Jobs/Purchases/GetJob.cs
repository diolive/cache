﻿using System;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasAnyRights]
	public class GetJob : Job<Purchase?>
	{
		private readonly Guid _purchaseId;

		public GetJob(Guid purchaseId)
		{
			_purchaseId = purchaseId;
		}

		protected override async Task<Purchase?> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Purchases.GetAsync(_purchaseId);
		}
	}
}