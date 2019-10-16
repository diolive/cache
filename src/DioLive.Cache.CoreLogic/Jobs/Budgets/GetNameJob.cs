﻿using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasAnyRights]
	public class GetNameJob : Job<string>
	{
		protected override async Task<string> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			Budget budget = await storageCollection.Budgets.GetAsync(CurrentBudget);

			return budget.Name;
		}
	}
}