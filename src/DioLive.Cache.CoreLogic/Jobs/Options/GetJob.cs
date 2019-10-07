using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Options
{
	public class GetJob : Job<Storage.Entities.Options>
	{
		protected override void Validation()
		{
			AssertUserIsAuthenticated();
		}

		protected override async Task<Storage.Entities.Options> ExecuteAsync()
		{
			IStorageCollection storageCollections = Settings.StorageCollection;

			Storage.Entities.Options? options = await storageCollections.Options.GetAsync();
			if (options is null)
			{
				options = GetDefaultOptions();
				await storageCollections.Options.CreateAsync(options);
			}

			return options;
		}

		private Storage.Entities.Options GetDefaultOptions()
		{
			return new Storage.Entities.Options
			{
				UserId = CurrentContext.UserId,
				PurchaseGrouping = 2,
				ShowPlanList = true
			};
		}
	}
}