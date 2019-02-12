using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICategoriesStorage
	{
		Task<(Result, Category)> GetAsync(int id);
		Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId, string culture = null);
		Task<IReadOnlyCollection<Category>> GetRootsAsync(Guid budgetId, string culture = null);
		Task<int> GetMostPopularIdAsync(Guid budgetId);
		Task InitializeCategoriesAsync(Guid budgetId);
		Task<int> AddAsync(string name, Guid budgetId);
		Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color);
		Task<Result> RemoveAsync(int id);
		Task<int?> GetLatestAsync(string purchase);
		Task<IReadOnlyCollection<Category>> GetChildrenAsync(int categoryId);
		Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId);
		Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, string uiCulture, int days = 0);
	}
}