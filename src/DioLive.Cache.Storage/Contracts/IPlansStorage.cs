using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPlansStorage
	{
		Task<Plan> FindAsync(int planId);
		Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId);
		Task BuyAsync(int planId);
		Task<Plan> AddAsync(string name, Guid budgetId);
		Task RemoveAsync(int planId);
	}
}