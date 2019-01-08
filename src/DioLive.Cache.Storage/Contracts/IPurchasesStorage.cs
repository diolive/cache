using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPurchasesStorage
	{
		Task<(Result, Purchase)> GetAsync(Guid id);
		Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, Func<Purchase, bool> filter);
		Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string shop, string comments);
		Task<Result> UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string shop, string comments);
		Task<Result> RemoveAsync(Guid id);
		Task<List<string>> GetShopsAsync(Guid budgetId);
		Task<List<string>> GetNamesAsync(Guid budgetId, string filter);
	}
}