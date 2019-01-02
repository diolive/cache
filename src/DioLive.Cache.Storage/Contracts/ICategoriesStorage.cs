using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICategoriesStorage
	{
		Task<(Result, Category)> GetAsync(int id);
		Task<List<Category>> GetAllAsync(Guid budgetId);
		Task<int> GetMostPopularIdAsync(Guid budgetId);
		Task InitializeCategoriesAsync(Guid budgetId);
		Task<int> AddAsync(string name, Guid budgetId);
		Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color);
		Task<Result> RemoveAsync(int id);
		Task<int?> GetLatestAsync(string purchase);
	}
}