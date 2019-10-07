using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Options
{
	public class UpdateJob : Job
	{
		private readonly int? _purchaseGrouping;
		private readonly bool? _showPlanList;

		public UpdateJob(int? purchaseGrouping, bool? showPlanList)
		{
			_purchaseGrouping = purchaseGrouping;
			_showPlanList = showPlanList;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollections = Settings.StorageCollection;

			await storageCollections.Options.UpdateAsync(_purchaseGrouping, _showPlanList);
		}
	}
}