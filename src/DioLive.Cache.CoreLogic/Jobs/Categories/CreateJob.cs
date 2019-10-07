using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class CreateJob : Job<int>
	{
		private readonly string _categoryName;

		public CreateJob(string categoryName)
		{
			_categoryName = categoryName;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.Categories);
		}

		protected override async Task<int> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.AddAsync(_categoryName, CurrentBudget);
		}
	}
}