using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class OpenJob : Job
	{
		private readonly Guid _budgetId;

		public OpenJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.ReadOnly);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			byte version = await storageCollection.Budgets.GetVersionAsync(_budgetId);
			if (version == 1)
			{
				await storageCollection.Categories.CloneCommonCategories(CurrentContext.UserId, _budgetId);
				await storageCollection.Budgets.SetVersionAsync(_budgetId, 2);
			}
		}
	}
}