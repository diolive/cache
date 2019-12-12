using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICategoriesStorage
	{
		Task<Category?> GetAsync(int id);
		Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId);
		Task<int?> GetMostPopularIdAsync(Guid budgetId);
		Task InitializeCategoriesAsync(Guid budgetId);
		Task<int> AddAsync(string name, Guid budgetId);
		Task UpdateAsync(int id, int? parentId, string name, string color);
		Task DeleteAsync(int id);
		Task<int?> GetLatestAsync(Guid budgetId, string purchase);
		Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, int days);
		Task CloneCommonCategories(string userId, Guid budgetId);
	}
}