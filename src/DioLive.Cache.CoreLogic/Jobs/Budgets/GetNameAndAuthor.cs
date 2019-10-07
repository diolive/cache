using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class GetNameAndAuthorJob : Job<(string name, string authorId)>
	{
		private readonly Guid _budgetId;

		public GetNameAndAuthorJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.ReadOnly);
		}

		protected override async Task<(string name, string authorId)> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			Budget budget = await storageCollection.Budgets.GetAsync(_budgetId);

			return (name: budget.Name, authorId: budget.AuthorId);
		}
	}
}