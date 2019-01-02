using System;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPlansStorage
	{
		Task<Plan> FindAsync(Guid budgetId, int planId);
		Task BuyAsync(Guid budgetId, int planId, string buyerId);
		Task<Plan> AddAsync(Guid budgetId, string name, string authorId);
		Task RemoveAsync(Guid budgetId, int planId);
	}
}