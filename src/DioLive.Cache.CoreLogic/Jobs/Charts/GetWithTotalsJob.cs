using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Charts
{
	[Authenticated]
	[HasAnyRights]
	public class GetWithTotalsJob : Job<IReadOnlyCollection<CategoryWithTotals>>
	{
		private readonly int _days;

		public GetWithTotalsJob(int days)
		{
			_days = days;
		}

		protected override async Task<IReadOnlyCollection<CategoryWithTotals>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetWithTotalsAsync(CurrentBudget, CurrentContext.Culture, _days);
		}
	}
}