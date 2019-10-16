using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasRights(ShareAccess.Categories)]
	public class DeleteJob : Job
	{
		private readonly int _categoryId;

		public DeleteJob(int categoryId)
		{
			_categoryId = categoryId;
		}

		protected override void CustomValidation()
		{
			AssertCategoryIsInCurrentBudget(_categoryId);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Categories.DeleteAsync(_categoryId);
		}
	}
}