using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICategoriesStorage
	{
		Task<(Result, Category)> GetAsync(int id);
		Task<IReadOnlyCollection<Category>> GetAllAsync(string culture = null);
		Task<IReadOnlyCollection<Category>> GetRootsAsync(string culture = null);
		Task<int?> GetMostPopularIdAsync();
		Task InitializeCategoriesAsync();
		Task<int> AddAsync(string name);
		Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color);
		Task<Result> RemoveAsync(int id);
		Task<int?> GetLatestAsync(string purchase);
		Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId);
		Task<CategoryWithTotals[]> GetWithTotalsAsync(string uiCulture, int days = 0);
	}
}