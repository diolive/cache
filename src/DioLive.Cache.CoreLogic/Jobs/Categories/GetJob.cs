using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class GetJob : Job<Category>
	{
		private readonly int _categoryId;

		public GetJob(int categoryId)
		{
			_categoryId = categoryId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForCategory(_categoryId, ShareAccess.ReadOnly);
		}

		protected override async Task<Category> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetAsync(_categoryId);
		}
	}
}