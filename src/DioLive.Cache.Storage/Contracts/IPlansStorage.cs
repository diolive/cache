using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPlansStorage
	{
		Task<Plan> FindAsync(Guid budgetId, int planId);
		Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId);
		Task BuyAsync(Guid budgetId, int planId);
		Task<Plan> AddAsync(Guid budgetId, string name);
		Task RemoveAsync(Guid budgetId, int planId);
	}
}