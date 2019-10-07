using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	public class GetAllJob : Job<IReadOnlyCollection<Category>>
	{
		private readonly string _culture;

		public GetAllJob(string culture)
		{
			_culture = culture;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<IReadOnlyCollection<Category>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Categories.GetAllAsync(CurrentBudget, _culture);
		}
	}
}