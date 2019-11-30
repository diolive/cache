using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasAnyRights]
	public class GetCurrencySignJob : Job<string>
	{
		protected override async Task<string> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			string currency = await storageCollection.Budgets.GetCurrencyAsync(CurrentBudget);

			return currency;
		}
	}
}