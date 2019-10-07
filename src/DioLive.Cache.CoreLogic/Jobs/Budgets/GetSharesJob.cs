using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class GetSharesJob : Job<IReadOnlyCollection<Share>>
	{
		private readonly Guid _budgetId;

		public GetSharesJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.Manage);
		}

		protected override async Task<IReadOnlyCollection<Share>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Budgets.GetSharesAsync(_budgetId);
		}
	}
}