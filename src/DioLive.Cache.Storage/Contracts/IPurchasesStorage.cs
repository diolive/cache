using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPurchasesStorage
	{
		Task<Purchase?> GetAsync(Guid id);
		Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, string? filter);
		Task<IReadOnlyCollection<Purchase>> GetForStatAsync(Guid budgetId, DateTime dateFrom, DateTime dateTo);
		Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string? shop, string? comments);
		Task UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string? shop, string? comments);
		Task RemoveAsync(Guid id);
		Task<IReadOnlyCollection<string>> GetShopsAsync(Guid budgetId);
		Task<IReadOnlyCollection<string>> GetNamesAsync(Guid budgetId, string filter);
	}
}