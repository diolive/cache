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
	public class BudgetsStorage : IBudgetsStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public BudgetsStorage(Func<SqlConnection> connectionAccessor,
		                      ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<(Result, Budget)> GetAsync(Guid id, ShareAccess requiredAccess)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				string userId = _currentContext.UserId;
				Result rights = await CheckUserRights(id, userId, requiredAccess, connection);

				if (rights != Result.Success)
				{
					return (rights, default);
				}

				Budget budget = await connection.QuerySingleOrDefaultAsync<Budget>(Queries.Budgets.Select, new { Id = id });
				return (Result.Success, budget);
			}
		}

		public async Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync()
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				string userId = _currentContext.UserId;
				return (await connection.QueryAsync<Budget>(Queries.Budgets.SelectAvailable, new { UserId = userId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<Guid> AddAsync(string name)
		{
			Guid budgetId = Guid.NewGuid();

			var budget = new Budget
			{
				Id = budgetId,
				Name = name,
				AuthorId = _currentContext.UserId,
				Version = 2
			};

			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Budgets.Insert, budget);
			}

			return budgetId;
		}

		public async Task<Result> RenameAsync(Guid id, string name)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await CheckUserRights(id, userId, ShareAccess.Manage, connection);

				if (rights == Result.Success)
				{
					await connection.ExecuteAsync(Queries.Budgets.Rename, new { Id = id, Name = name });
				}

				return rights;
			}
		}

		public async Task<Result> RemoveAsync(Guid id)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await CheckUserRights(id, userId, ShareAccess.Delete, connection);

				if (rights == Result.Success)
				{
					await connection.ExecuteAsync(Queries.Budgets.Delete, new { Id = id });
				}

				return rights;
			}
		}

		public async Task<Result> ShareAsync(Guid id, string userId, ShareAccess access)
		{
			string currentUserId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await CheckUserRights(id, currentUserId, ShareAccess.Manage, connection);

				if (rights != Result.Success)
				{
					return rights;
				}

				var share = new Share
				{
					BudgetId = id,
					UserId = userId,
					Access = access
				};

				await connection.ExecuteAsync(Queries.Budgets.Share, share);

				return Result.Success;
			}
		}

		public async Task<Result> MigrateAsync(Guid id)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				int budgetVersion = await connection.ExecuteScalarAsync<int>(Queries.Budgets.GetVersion, new { Id = id });
				if (budgetVersion != 1)
				{
					return Result.Error;
				}

				string userId = _currentContext.UserId;
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
					category.BudgetId = id;
					category.Id = 0;

					int newId = await connection.ExecuteScalarAsync<int>(Queries.Categories.Insert, category);

					ReadOnlyCollection<Category> children = commonCategories
						.Where(c => c.ParentId == oldId)
						.ToList()
						.AsReadOnly();

					foreach (Category child in children)
					{
						child.ParentId = newId;
						await CloneCategory(child);
					}

					await connection.ExecuteAsync(Queries.Purchases.UpdateCategory, new { BudgetId = id, OldCategoryId = oldId, NewCategoryId = newId });
				}

				await connection.ExecuteAsync(Queries.Budgets.SetVersion, new { BudgetId = id, Version = 2 });

				return Result.Success;
			}
		}

		public async Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<Share>(Queries.Budgets.GetShares, new { BudgetId = budgetId }))
					.ToList()
					.AsReadOnly();
			}
		}

		private static async Task<Result> CheckUserRights(Guid budgetId, string userId, ShareAccess requiredAccess, SqlConnection connection)
		{
			return await connection.ExecuteScalarAsync<Result>(Queries.Budgets.CheckRights, new { BudgetId = budgetId, UserId = userId, Access = requiredAccess });
		}
	}
}