using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	public class CreateJob : Job<Plan>
	{
		private readonly string _name;

		public CreateJob(string name)
		{
			_name = name;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.Purchases);
		}

		protected override async Task<Plan> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Plans.AddAsync(_name, CurrentBudget);
		}
	}
}