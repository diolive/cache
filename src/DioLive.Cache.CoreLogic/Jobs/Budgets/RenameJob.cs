using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class RenameJob : Job
	{
		private readonly Guid _budgetId;
		private readonly string _newName;

		public RenameJob(Guid budgetId, string newName)
		{
			_budgetId = budgetId;
			_newName = newName;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.Manage);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Budgets.RenameAsync(_budgetId, _newName);
		}
	}
}