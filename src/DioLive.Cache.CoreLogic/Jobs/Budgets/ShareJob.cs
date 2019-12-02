using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasRights(ShareAccess.Manage)]
	public class ShareJob : Job
	{
		private readonly ShareAccess _targetAccess;
		private readonly string _targetUserName;

		public ShareJob(string targetUserName, ShareAccess targetAccess)
		{
			_targetUserName = targetUserName;
			_targetAccess = targetAccess;
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			string? userId = await storageCollection.Users.FindIdByNameAsync(_targetUserName);
			if (userId is null)
			{
				ValidationException.RaiseIfNeeded(ResultStatus.NotFound);
			}

			await storageCollection.Budgets.ShareAsync(CurrentBudget, userId, _targetAccess);
		}
	}
}