using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPlansStorage
	{
		Task<Plan> FindAsync(int planId);
		Task<IReadOnlyCollection<Plan>> FindAllAsync();
		Task BuyAsync(int planId);
		Task<Plan> AddAsync(string name);
		Task RemoveAsync(int planId);
	}
}