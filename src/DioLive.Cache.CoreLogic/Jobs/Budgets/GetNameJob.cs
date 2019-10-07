using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class GetNameJob : Job<string>
	{
		private readonly Guid _budgetId;

		public GetNameJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.ReadOnly);
		}

		protected override async Task<string> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			Budget budget = await storageCollection.Budgets.GetAsync(_budgetId);

			return budget.Name;
		}
	}
}