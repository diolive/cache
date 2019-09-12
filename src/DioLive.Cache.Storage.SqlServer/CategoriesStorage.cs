using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class CategoriesStorage : StorageBase, ICategoriesStorage
	{
		public CategoriesStorage(Func<IDbConnection> connectionAccessor,
		                         ICurrentContext currentContext)
			: base(connectionAccessor, currentContext)
		{
		}

		public async Task<(Result, Category)> GetAsync(int id)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, CurrentUserId, ShareAccess.ReadOnly, connection);

				if (rights != Result.Success)
				{
					return (rights, default);
				}

				Category category = await connection.QuerySingleOrDefaultAsync<Category>(Queries.Categories.Select, new { Id = id });

				return (Result.Success, category);
			}
		}

		public async Task<IReadOnlyCollection<Category>> GetAllAsync(string culture = null)
		{
			using (IDbConnection connection = OpenConnection())
			{
				await CheckIfUserHasAnyRightsOnBudget(CurrentBudgetId, CurrentUserId, connection);

				string query = culture is null
					? Queries.Categories.SelectAll
					: Queries.Categories.SelectAllWithLocalNames;

				return (await connection.QueryAsync<Category>(query, new { BudgetId = CurrentBudgetId, Culture = culture }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<IReadOnlyCollection<Category>> GetRootsAsync(string culture = null)
		{
			using (IDbConnection connection = OpenConnection())
			{
				await CheckIfUserHasAnyRightsOnBudget(CurrentBudgetId, CurrentUserId, connection);

				string query = culture is null
					? Queries.Categories.SelectAllRoots
					: Queries.Categories.SelectAllRootsWithLocalNames;

				return (await connection.QueryAsync<Category>(query, new { BudgetId = CurrentBudgetId, Culture = culture }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<int?> GetMostPopularIdAsync()
		{
			using (IDbConnection connection = OpenConnection())
			{
				await CheckIfUserHasAnyRightsOnBudget(CurrentBudgetId, CurrentUserId, connection);

				return await connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.SelectMostPopularId, new { BudgetId = CurrentBudgetId });
			}
		}

		public async Task InitializeCategoriesAsync()
		{
			using (IDbConnection connection = OpenConnection())
			{
				await CloneCommonCategories(CurrentUserId, CurrentBudgetId, connection);
			}
		}

		public async Task<int> AddAsync(string name)
		{
			using (IDbConnection connection = OpenConnection())
			{
				await CheckIfUserHasRightsOnBudget(CurrentBudgetId, CurrentUserId, ShareAccess.Categories, connection);

				var category = new Category
				{
					Name = name,
					BudgetId = CurrentBudgetId,
					OwnerId = CurrentUserId
				};

				return await connection.ExecuteScalarAsync<int>(Queries.Categories.Insert, category);
			}
		}

		public async Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, CurrentUserId, ShareAccess.Categories, connection);
				if (rights != Result.Success)
				{
					return rights;
				}

				await connection.ExecuteAsync(Queries.Categories.Update, new { Id = id, Name = translates[0].name, ParentId = parentId, Color = color });
				await connection.ExecuteAsync(Queries.Categories.DeleteLocalizations, new { CategoryId = id });
				foreach ((string name, string culture) in translates)
				{
					await connection.ExecuteAsync(Queries.Categories.InsertLocalization, new { CategoryId = id, Culture = culture, Name = name });
				}

				return Result.Success;
			}
		}

		public async Task<Result> RemoveAsync(int id)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, CurrentUserId, ShareAccess.Categories, connection);
				if (rights != Result.Success)
				{
					return rights;
				}

				await connection.ExecuteAsync(Queries.Categories.Delete, new { Id = id });
			}

			return Result.Success;
		}

		public async Task<int?> GetLatestAsync(string purchase)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.GetLatest, new { BudgetId = CurrentBudgetId, Name = purchase });
			}
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return (await connection.QueryAsync<CategoryLocalization>(Queries.Categories.GetLocalizations, new { CategoryId = categoryId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<CategoryWithTotals[]> GetWithTotalsAsync(string uiCulture, int days = 0)
		{
			IReadOnlyCollection<Category> categories = await GetAllAsync(uiCulture);
			IEnumerable<Category> rootCategories = categories.Where(c => !c.ParentId.HasValue);

			using (IDbConnection connection = OpenConnection())
			{
				ReadOnlyCollection<CategoryWithTotals> categoriesWithTotal = (await connection.QueryAsync<CategoryWithTotals>(Queries.Categories.GetWithTotals, new { BudgetId = CurrentBudgetId, Culture = uiCulture, Days = days }))
					.ToList()
					.AsReadOnly();

				foreach (CategoryWithTotals categoryWithTotals in categoriesWithTotal)
				{
					categoryWithTotals.Children = categories
						.Where(c => c.ParentId == categoryWithTotals.Id)
						.Select(c => categoriesWithTotal.Single(ct => ct.Id == c.Id))
						.ToList()
						.AsReadOnly();
				}

				return rootCategories
					.Select(rc => categoriesWithTotal.Single(ct => ct.Id == rc.Id))
					.ToArray();
			}
		}

		internal static async Task CloneCommonCategories(string userId, Guid budgetId, IDbConnection connection)
		{
			ReadOnlyCollection<Category> commonCategories = (await connection.QueryAsync<Category>(Queries.Categories.SelectCommon))
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

				int newId = await connection.ExecuteScalarAsync<int>(Queries.Categories.Clone, category);

				ReadOnlyCollection<Category> children = commonCategories
					.Where(c => c.ParentId == oldId)
					.ToList()
					.AsReadOnly();

				foreach (Category child in children)
				{
					child.ParentId = newId;
					await CloneCategory(child);
				}

				await connection.ExecuteAsync(Queries.Purchases.UpdateCategory, new { BudgetId = budgetId, OldCategoryId = oldId, NewCategoryId = newId });
			}
		}

		private static async Task CheckIfUserHasAnyRightsOnBudget(Guid budgetId, string userId, IDbConnection connection)
		{
			await CheckIfUserHasRightsOnBudget(budgetId, userId, ShareAccess.ReadOnly, connection);
		}

		private static async Task CheckIfUserHasRightsOnBudget(Guid budgetId, string userId, ShareAccess expectedAccess, IDbConnection connection)
		{
			Result rights = await PermissionsValidator.CheckUserRightsForBudget(budgetId, userId, expectedAccess, connection);

			if (rights != Result.Success)
			{
				throw new InvalidOperationException($"Cannot perform this operation: {rights}");
			}
		}
	}
}