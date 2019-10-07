using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class CategoriesStorage : StorageBase, ICategoriesStorage
	{
		public CategoriesStorage(IConnectionInfo connectionInfo,
		                         ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task<Category> GetAsync(int id)
		{
			return await Connection.QuerySingleOrDefaultAsync<Category>(Queries.Categories.Select, new { Id = id });
		}

		public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId, string? culture = null)
		{
			string query = culture is null
				? Queries.Categories.SelectAll
				: Queries.Categories.SelectAllWithLocalNames;

			return (await Connection.QueryAsync<Category>(query, new { BudgetId = budgetId, Culture = culture }))
				.ToList()
				.AsReadOnly();
		}

		public async Task<int?> GetMostPopularIdAsync(Guid budgetId)
		{
			return await Connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.SelectMostPopularId, new { BudgetId = budgetId });
		}

		public async Task InitializeCategoriesAsync(Guid budgetId)
		{
			await CloneCommonCategories(CurrentUserId, budgetId);
		}

		public async Task<int> AddAsync(string name, Guid budgetId)
		{
			var category = new Category
			{
				Name = name,
				BudgetId = budgetId,
				OwnerId = CurrentUserId,
				Color = GetRandomColor()
			};

			return await Connection.ExecuteScalarAsync<int>(Queries.Categories.Insert, category);

			int GetRandomColor()
			{
				return Guid.NewGuid().ToByteArray().Take(3).Select((b, index) => b << (8 * index)).Sum();
			}
		}

		public async Task UpdateAsync(int id, int? parentId, LocalizedName[] translates, string color)
		{
			await Connection.ExecuteAsync(Queries.Categories.Update, new { Id = id, translates[0].Name, ParentId = parentId, Color = color });
			await Connection.ExecuteAsync(Queries.Categories.DeleteLocalizations, new { CategoryId = id });
			foreach (LocalizedName localizedName in translates)
			{
				await Connection.ExecuteAsync(Queries.Categories.InsertLocalization, new { CategoryId = id, localizedName.Culture, localizedName.Name });
			}
		}

		public async Task DeleteAsync(int id)
		{
			await Connection.ExecuteAsync(Queries.Categories.Delete, new { Id = id });
		}

		public async Task<int?> GetLatestAsync(Guid budgetId, string purchase)
		{
			return await Connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.GetLatest, new { BudgetId = budgetId, Name = purchase });
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			return (await Connection.QueryAsync<CategoryLocalization>(Queries.Categories.GetLocalizations, new { CategoryId = categoryId }))
				.ToList()
				.AsReadOnly();
		}

		public async Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, string uiCulture, int days = 0)
		{
			IReadOnlyCollection<Category> categories = await GetAllAsync(budgetId, uiCulture);
			IEnumerable<Category> rootCategories = categories.Where(c => !c.ParentId.HasValue);

			ReadOnlyCollection<CategoryWithTotals> categoriesWithTotal = (await Connection.QueryAsync<CategoryWithTotals>(Queries.Categories.GetWithTotals, new { BudgetId = budgetId, Culture = uiCulture, Days = days }))
				.ToList()
				.AsReadOnly();

			foreach (CategoryWithTotals categoryWithTotals in categoriesWithTotal)
			{
				categoryWithTotals.Children = categories
					.Where(c => c.ParentId == categoryWithTotals.Id)
					.Select(c => categoriesWithTotal.SingleOrDefault(ct => ct.Id == c.Id))
					.Where(c => c != null)
					.ToList()
					.AsReadOnly();
			}

			return rootCategories
				.Select(rc => categoriesWithTotal.SingleOrDefault(ct => ct.Id == rc.Id))
				.Where(c => c != null)
				.ToArray();
		}

		public async Task CloneCommonCategories(string userId, Guid budgetId)
		{
			ReadOnlyCollection<Category> commonCategories = (await Connection.QueryAsync<Category>(Queries.Categories.SelectCommon))
				.ToList()
				.AsReadOnly();

			ReadOnlyCollection<Category> rootCategories = commonCategories.Where(c => !c.ParentId.HasValue)
				.ToList()
				.AsReadOnly();

			foreach (Category rootCategory in rootCategories)
			{
				await CloneCategory(rootCategory);
			}

			async Task CloneCategory(Category category)
			{
				int oldId = category.Id;

				category.OwnerId = userId;
				category.BudgetId = budgetId;

				int newId = await Connection.ExecuteScalarAsync<int>(Queries.Categories.Clone, category);

				ReadOnlyCollection<Category> children = commonCategories
					.Where(c => c.ParentId == oldId)
					.ToList()
					.AsReadOnly();

				foreach (Category child in children)
				{
					child.ParentId = newId;
					await CloneCategory(child);
				}

				await Connection.ExecuteAsync(Queries.Purchases.UpdateCategory, new { BudgetId = budgetId, OldCategoryId = oldId, NewCategoryId = newId });
			}
		}
	}
}