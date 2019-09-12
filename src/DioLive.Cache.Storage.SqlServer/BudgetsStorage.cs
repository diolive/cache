using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class BudgetsStorage : StorageBase, IBudgetsStorage
	{
		public BudgetsStorage(Func<IDbConnection> connectionAccessor,
		                      ICurrentContext currentContext)
			: base(connectionAccessor, currentContext)
		{
		}

		public async Task<(Result, Budget)> GetAsync(Guid id, ShareAccess requiredAccess)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForBudget(id, CurrentUserId, requiredAccess, connection);

				if (rights != Result.Success)
				{
					return (rights, default);
				}

				Budget budget = await connection.QuerySingleOrDefaultAsync<Budget>(Queries.Budgets.Select, new { Id = id });
				return (Result.Success, budget);
			}
		}

		public async Task<Result> CheckAccessAsync(Guid id, ShareAccess requiredAccess)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return await PermissionsValidator.CheckUserRightsForBudget(id, CurrentUserId, requiredAccess, connection);
			}
		}

		public async Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync()
		{
			using (IDbConnection connection = OpenConnection())
			{
				return (await connection.QueryAsync<Budget>(Queries.Budgets.SelectAvailable, new { UserId = CurrentUserId }))
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
				AuthorId = CurrentUserId,
				Version = 2
			};

			using (IDbConnection connection = OpenConnection())
			{
				await connection.ExecuteAsync(Queries.Budgets.Insert, budget);
			}

			return budgetId;
		}

		public async Task<Result> RenameAsync(Guid id, string name)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForBudget(id, CurrentUserId, ShareAccess.Manage, connection);

				if (rights == Result.Success)
				{
					await connection.ExecuteAsync(Queries.Budgets.Rename, new { Id = id, Name = name });
				}

				return rights;
			}
		}

		public async Task<Result> RemoveAsync(Guid id)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForBudget(id, CurrentUserId, ShareAccess.Delete, connection);

				if (rights == Result.Success)
				{
					await connection.ExecuteAsync(Queries.Budgets.Delete, new { Id = id });
				}

				return rights;
			}
		}

		public async Task<Result> ShareAsync(Guid id, string userId, ShareAccess access)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForBudget(id, CurrentUserId, ShareAccess.Manage, connection);

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
			using (IDbConnection connection = OpenConnection())
			{
				int budgetVersion = await connection.ExecuteScalarAsync<int>(Queries.Budgets.GetVersion, new { Id = id });
				if (budgetVersion != 1)
				{
					return Result.Error;
				}

				string userId = CurrentUserId;
				await CategoriesStorage.CloneCommonCategories(userId, id, connection);
				await connection.ExecuteAsync(Queries.Budgets.SetVersion, new { BudgetId = id, Version = 2 });

				return Result.Success;
			}
		}

		public async Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return (await connection.QueryAsync<Share>(Queries.Budgets.GetShares, new { BudgetId = budgetId }))
					.ToList()
					.AsReadOnly();
			}
		}
	}
}