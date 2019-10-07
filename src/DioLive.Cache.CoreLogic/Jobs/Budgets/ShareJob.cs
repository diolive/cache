using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	public class ShareJob : Job
	{
		private readonly ShareAccess _targetAccess;
		private readonly string _targetUserId;

		public ShareJob(string targetUserId, ShareAccess targetAccess)
		{
			_targetUserId = targetUserId;
			_targetAccess = targetAccess;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.Manage);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Budgets.ShareAsync(CurrentBudget, _targetUserId, _targetAccess);
		}
	}
}