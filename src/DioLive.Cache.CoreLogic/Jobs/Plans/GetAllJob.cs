using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	[Authenticated]
	[HasAnyRights]
	public class GetAllJob : Job<IReadOnlyCollection<Plan>>
	{
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