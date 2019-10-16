using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	[Authenticated]
	[HasRights(ShareAccess.Purchases)]
	public class DeleteJob : Job
	{
		private readonly int _planId;

		public DeleteJob(int planId)
		{
			_planId = planId;
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Plans.RemoveAsync(_planId);
		}
	}
}