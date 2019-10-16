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
	public class PlansStorage : StorageBase, IPlansStorage
	{
		public PlansStorage(IConnectionInfo connectionInfo,
		                    ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task<Plan> FindAsync(int planId)
		{
			return await Connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId });
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId)
		{
			return (await Connection.QueryAsync<Plan>(Queries.Plans.SelectAll, new { BudgetId = budgetId }))
				.ToList()
				.AsReadOnly();
		}

		public async Task BuyAsync(int planId)
		{
			Plan plan = await Connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId });

			if (plan is null)
			{
				return;
			}

			plan.BuyDate = DateTime.UtcNow;
			plan.BuyerId = CurrentUserId;

			await Connection.ExecuteAsync(Queries.Plans.Buy, plan);
		}

		public async Task<Plan> AddAsync(string name, Guid budgetId)
		{
			var plan = new Plan
			{
				Name = name,
				AuthorId = CurrentUserId,
				BudgetId = budgetId
			};

			plan.Id = await Connection.ExecuteScalarAsync<int>(Queries.Plans.Insert, plan);

			return plan;
		}

		public async Task RemoveAsync(int planId)
		{
			await Connection.ExecuteAsync(Queries.Plans.Delete, new { Id = planId });
		}
	}
}