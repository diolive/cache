using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Options
{
	[Authenticated]
	public class GetJob : Job<Common.Entities.Options>
	{
		protected override async Task<Common.Entities.Options> ExecuteAsync()
		{
			IStorageCollection storageCollections = Settings.StorageCollection;

			Common.Entities.Options? options = await storageCollections.Options.GetAsync();
			if (options is null)
			{
				options = GetDefaultOptions();
				await storageCollections.Options.CreateAsync(options);
			}

			return options;
		}

		private Common.Entities.Options GetDefaultOptions()
		{
			return new Common.Entities.Options
			{
				UserId = CurrentContext.UserId,
				PurchaseGrouping = 2,
				ShowPlanList = true
			};
		}
	}
}