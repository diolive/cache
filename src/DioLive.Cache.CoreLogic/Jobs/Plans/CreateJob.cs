using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	[Authenticated]
	[HasRights(ShareAccess.Purchases)]
	public class CreateJob : Job<Plan>
	{
		private readonly string _name;

		public CreateJob(string name)
		{
			_name = name;
		}

		protected override async Task<Plan> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return await storageCollection.Plans.AddAsync(_name, CurrentBudget);
		}
	}
}