using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasRights(ShareAccess.Categories)]
	public class CreateJob : Job<int>
	{
		private readonly string _categoryName;

		public CreateJob(string categoryName)
		{
			_categoryName = categoryName;
		}

		protected override async Task<int> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.AddAsync(_categoryName, CurrentBudget);
		}
	}
}