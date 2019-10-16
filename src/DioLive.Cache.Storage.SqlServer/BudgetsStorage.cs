using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class BudgetsStorage : StorageBase, IBudgetsStorage
	{
		public BudgetsStorage(IConnectionInfo connectionInfo,
		                      ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task<Budget> GetAsync(Guid id)
		{
			return await Connection.QuerySingleOrDefaultAsync<Budget>(Queries.Budgets.Select, new { Id = id });
		}

		public async Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync()
		{
			return (await Connection.QueryAsync<Budget>(Queries.Budgets.SelectAvailable, new { UserId = CurrentUserId }))
				.ToList()
				.AsReadOnly();
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

			await Connection.ExecuteAsync(Queries.Budgets.Insert, budget);

			return budgetId;
		}

		public async Task RenameAsync(Guid id, string name)
		{
			await Connection.ExecuteAsync(Queries.Budgets.Rename, new { Id = id, Name = name });
		}

		public async Task DeleteAsync(Guid id)
		{
			await Connection.ExecuteAsync(Queries.Budgets.Delete, new { Id = id });
		}

		public async Task ShareAsync(Guid id, string userId, ShareAccess access)
		{
			var share = new Share
			{
				BudgetId = id,
				UserId = userId,
				Access = access
			};

			await Connection.ExecuteAsync(Queries.Budgets.Share, share);
		}

		public async Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId)
		{
			return (await Connection.QueryAsync<Share>(Queries.Budgets.GetShares, new { BudgetId = budgetId }))
				.ToList()
				.AsReadOnly();
		}

		public async Task<byte> GetVersionAsync(Guid id)
		{
			return await Connection.ExecuteScalarAsync<byte>(Queries.Budgets.GetVersion, new { Id = id });
		}

		public async Task SetVersionAsync(Guid id, byte version)
		{
			await Connection.ExecuteAsync(Queries.Budgets.SetVersion, new { Id = id, Version = (byte) 2 });
		}
	}
}