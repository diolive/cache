using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPurchasesStorage
	{
		Task<(Result, Purchase)> GetAsync(Guid id);
		Task<List<Purchase>> FindAsync(Guid budgetId, string filter);
		Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string shop, string comments);
		Task<Result> UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string shop, string comments);
		Task<Result> RemoveAsync(Guid id);
		Task<List<string>> GetShopsAsync(Guid budgetId);
		Task<List<string>> GetNamesAsync(Guid budgetId, string filter);
	}
}