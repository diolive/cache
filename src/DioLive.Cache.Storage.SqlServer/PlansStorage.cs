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
	public class PlansStorage : StorageBase, IPlansStorage
	{
		public PlansStorage(Func<IDbConnection> connectionAccessor,
		                    ICurrentContext currentContext)
			: base(connectionAccessor, currentContext)
		{
		}

		public async Task<Plan> FindAsync(int planId)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = CurrentBudgetId });
			}
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync()
		{
			using (IDbConnection connection = OpenConnection())
			{
				return (await connection.QueryAsync<Plan>(Queries.Plans.SelectAll, new { BudgetId = CurrentBudgetId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task BuyAsync(int planId)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Plan plan = await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = CurrentBudgetId });

				if (plan is null)
				{
					return;
				}

				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = CurrentUserId;

				await connection.ExecuteAsync(Queries.Plans.Buy, plan);
			}
		}

		public async Task<Plan> AddAsync(string name)
		{
			var plan = new Plan
			{
				Name = name,
				AuthorId = CurrentUserId,
				BudgetId = CurrentBudgetId
			};

			using (IDbConnection connection = OpenConnection())
			{
				int id = await connection.ExecuteScalarAsync<int>(Queries.Plans.Insert, plan);
				plan.Id = id;

				return plan;
			}
		}

		public async Task RemoveAsync(int planId)
		{
			using (IDbConnection connection = OpenConnection())
			{
				await connection.ExecuteAsync(Queries.Plans.Delete, new { Id = planId, BudgetId = CurrentBudgetId });
			}
		}
	}
}