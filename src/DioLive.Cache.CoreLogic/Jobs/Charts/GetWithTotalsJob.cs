using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Charts
{
	public class GetWithTotalsJob : Job<IReadOnlyCollection<CategoryWithTotals>>
	{
		private readonly int _days;

		public GetWithTotalsJob(int days)
		{
			_days = days;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<IReadOnlyCollection<CategoryWithTotals>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetWithTotalsAsync(CurrentBudget, CurrentContext.Culture, _days);
		}
	}
}