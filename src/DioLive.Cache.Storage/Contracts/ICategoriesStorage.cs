using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICategoriesStorage
	{
		Task<List<Category>> GetAsync(Guid budgetId);
		Task<int> GetMostPopularIdAsync(Guid budgetId);
		Task InitializeCategoriesAsync(Guid budgetId, string userId);
		Task AddAsync(Category category);
		Task<Result> UpdateAsync(int id, string userId, int? parentId, (string name, string culture)[] translates, string color);
		Task<(Result, Category)> GetForRemoveAsync(int id, string userId);
		Task<Result> RemoveAsync(int id, string userId);
		Task<int?> GetLatestAsync(string purchase);
	}
}