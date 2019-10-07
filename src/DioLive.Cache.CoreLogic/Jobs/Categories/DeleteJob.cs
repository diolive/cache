using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class DeleteJob : Job
	{
		private readonly int _categoryId;

		public DeleteJob(int categoryId)
		{
			_categoryId = categoryId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForCategory(_categoryId, ShareAccess.Categories);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Categories.DeleteAsync(_categoryId);
		}
	}
}