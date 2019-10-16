using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasAnyRights]
	public class GetPreviousJob : Job<int?>
	{
		private readonly string _purchaseName;

		public GetPreviousJob(string purchaseName)
		{
			_purchaseName = purchaseName;
		}

		protected override async Task<int?> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetLatestAsync(CurrentBudget, _purchaseName);
		}
	}
}