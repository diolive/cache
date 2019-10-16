using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasAnyRights]
	public class GetJob : Job<Category?>
	{
		private readonly int _categoryId;

		public GetJob(int categoryId)
		{
			_categoryId = categoryId;
		}

		protected override void CustomValidation()
		{
			AssertCategoryIsInCurrentBudget(_categoryId);
		}

		protected override async Task<Category?> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetAsync(_categoryId);
		}
	}
}