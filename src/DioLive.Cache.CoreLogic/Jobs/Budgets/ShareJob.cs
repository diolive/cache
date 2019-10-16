using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasRights(ShareAccess.Manage)]
	public class ShareJob : Job
	{
		private readonly ShareAccess _targetAccess;
		private readonly string _targetUserId;

		public ShareJob(string targetUserId, ShareAccess targetAccess)
		{
			_targetUserId = targetUserId;
			_targetAccess = targetAccess;
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Budgets.ShareAsync(CurrentBudget, _targetUserId, _targetAccess);
		}
	}
}