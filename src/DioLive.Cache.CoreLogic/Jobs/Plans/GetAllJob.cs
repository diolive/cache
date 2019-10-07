using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	public class GetAllJob : Job<IReadOnlyCollection<Plan>>
	{
		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.ReadOnly);
		}

		protected override async Task<IReadOnlyCollection<Plan>> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return (await storageCollection.Plans.FindAllAsync(CurrentBudget))
				.OrderBy(p => p.Name)
				.ToList()
				.AsReadOnly();
		}
	}
}