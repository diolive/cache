using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasAnyRights]
	public class GetNamesJob : Job<IReadOnlyCollection<string>>
	{
		private readonly string _filter;

		public GetNamesJob(string filter)
		{
			_filter = filter;
		}

		protected override async Task<IReadOnlyCollection<string>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Purchases.GetNamesAsync(CurrentBudget, _filter);
		}
	}
}