using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class DeleteJob : Job
	{
		private readonly Guid _budgetId;

		public DeleteJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.Delete);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Budgets.DeleteAsync(_budgetId);
		}
	}
}