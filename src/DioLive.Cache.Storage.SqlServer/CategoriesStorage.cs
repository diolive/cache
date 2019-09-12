using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class CategoriesStorage : ICategoriesStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public CategoriesStorage(Func<SqlConnection> connectionAccessor,
		                         ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<(Result, Category)> GetAsync(int id)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, userId, ShareAccess.ReadOnly, connection);

				if (rights != Result.Success)
				{
					return (rights, default);
				}

				Category category = await connection.QuerySingleOrDefaultAsync<Category>(Queries.Categories.Select, new { Id = id });

				return (Result.Success, category);
			}
		}

		public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId, string culture = null)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await CheckIfUserHasAnyRightsOnBudget(budgetId, _currentContext.UserId, connection);

				string query = culture is null
					? Queries.Categories.SelectAll
					: Queries.Categories.SelectAllWithLocalNames;

				return (await connection.QueryAsync<Category>(query, new { BudgetId = budgetId, Culture = culture }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<IReadOnlyCollection<Category>> GetRootsAsync(Guid budgetId, string culture = null)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await CheckIfUserHasAnyRightsOnBudget(budgetId, _currentContext.UserId, connection);

				string query = culture is null
					? Queries.Categories.SelectAllRoots
					: Queries.Categories.SelectAllRootsWithLocalNames;

				return (await connection.QueryAsync<Category>(query, new { BudgetId = budgetId, Culture = culture }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<int?> GetMostPopularIdAsync(Guid budgetId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await CheckIfUserHasAnyRightsOnBudget(budgetId, _currentContext.UserId, connection);

				return await connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.SelectMostPopularId, new { BudgetId = budgetId });
			}
		}

		public async Task InitializeCategoriesAsync(Guid budgetId)
		{
			string userid = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				await CloneCommonCategories(userid, budgetId, connection);
			}
		}

		public async Task<int> AddAsync(string name, Guid budgetId)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				await CheckIfUserHasRightsOnBudget(budgetId, userId, ShareAccess.Categories, connection);

				var category = new Category
				{
					Name = name,
					BudgetId = budgetId,
					OwnerId = userId
				};

				return await connection.ExecuteScalarAsync<int>(Queries.Categories.Insert, category);
			}
		}

		public async Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, userId, ShareAccess.Categories, connection);
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
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForCategory(id, userId, ShareAccess.Categories, connection);
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
			if (!_currentContext.BudgetId.HasValue)
			{
				return default;
			}

			Guid budgetId = _currentContext.BudgetId.Value;

			using (SqlConnection connection = _connectionAccessor())
			{
				return await connection.QuerySingleOrDefaultAsync<int?>(Queries.Categories.GetLatest, new { BudgetId = budgetId, Name = purchase });
			}
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<CategoryLocalization>(Queries.Categories.GetLocalizations, new { CategoryId = categoryId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, string uiCulture, int days = 0)
		{
			IReadOnlyCollection<Category> categories = await GetAllAsync(budgetId, uiCulture);
			IEnumerable<Category> rootCategories = categories.Where(c => !c.ParentId.HasValue);

			using (SqlConnection connection = _connectionAccessor())
			{
				ReadOnlyCollection<CategoryWithTotals> categoriesWithTotal = (await connection.QueryAsync<CategoryWithTotals>(Queries.Categories.GetWithTotals, new { BudgetId = budgetId, Culture = uiCulture, Days = days }))
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

		internal static async Task CloneCommonCategories(string userId, Guid budgetId, SqlConnection connection)
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

		private static async Task CheckIfUserHasAnyRightsOnBudget(Guid budgetId, string userId, SqlConnection connection)
		{
			await CheckIfUserHasRightsOnBudget(budgetId, userId, ShareAccess.ReadOnly, connection);
		}

		private static async Task CheckIfUserHasRightsOnBudget(Guid budgetId, string userId, ShareAccess expectedAccess, SqlConnection connection)
		{
			Result rights = await PermissionsValidator.CheckUserRightsForBudget(budgetId, userId, expectedAccess, connection);

			if (rights != Result.Success)
			{
				throw new InvalidOperationException($"Cannot perform this operation: {rights}");
			}
		}
	}
}