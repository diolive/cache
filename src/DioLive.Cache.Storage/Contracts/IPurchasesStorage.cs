using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPurchasesStorage
	{
		Task<List<Purchase>> FindAsync(Guid budgetId, string filter);
		Task<Purchase> GetWithSharesAsync(Guid id);
		Task AddAsync(Purchase purchase);
		Task<(Result, Purchase)> GetForModificationAsync(Guid id, string userId);
		Task<Result> UpdateAsync(Guid id, string userId, int categoryId, DateTime date, string name, int cost, string shop, string comments);
		Task<Result> RemoveAsync(Guid id, string userId);
		Task<List<string>> GetShopsAsync(Guid budgetId);
		Task<List<string>> GetNamesAsync(Guid budgetId, string q);
	}
}