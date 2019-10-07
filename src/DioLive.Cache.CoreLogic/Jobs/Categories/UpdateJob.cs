using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class UpdateJob : Job
	{
		private readonly int _categoryId;
		private readonly string _color;
		private readonly int? _parentCategoryId;
		private readonly LocalizedName[] _translates;

		public UpdateJob(int categoryId, int? parentCategoryId, LocalizedName[] translates, string color)
		{
			_categoryId = categoryId;
			_parentCategoryId = parentCategoryId;
			_translates = translates;
			_color = color;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForCategory(_categoryId, ShareAccess.Categories);
			if (_parentCategoryId.HasValue)
			{
				AssertUserHasAccessForCategory(_parentCategoryId.Value, ShareAccess.Categories);
			}
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Categories.UpdateAsync(_categoryId, _parentCategoryId, _translates, _color);
		}
	}
}