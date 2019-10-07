using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class GetPreviousJob : Job<int?>
	{
		private readonly string _purchaseName;

		public GetPreviousJob(string purchaseName)
		{
			_purchaseName = purchaseName;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<int?> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetLatestAsync(CurrentBudget, _purchaseName);
		}
	}
}