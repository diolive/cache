using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasRights(ShareAccess.Manage)]
	public class RenameJob : Job
	{
		private readonly string _newName;

		public RenameJob(string newName)
		{
			_newName = newName;
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Budgets.RenameAsync(CurrentBudget, _newName);
		}
	}
}