using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class PlansStorage : IPlansStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public PlansStorage(Func<SqlConnection> connectionAccessor,
		                    ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<Plan> FindAsync(Guid budgetId, int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = budgetId });
			}
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<Plan>(Queries.Plans.SelectAll, new { BudgetId = budgetId })).ToList().AsReadOnly();
			}
		}

		public async Task BuyAsync(Guid budgetId, int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				Plan plan = await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = budgetId });

				if (plan is null)
				{
					return;
				}

				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = _currentContext.UserId;

				await connection.ExecuteAsync(Queries.Plans.Buy, plan);
			}
		}

		public async Task<Plan> AddAsync(Guid budgetId, string name)
		{
			var plan = new Plan
			{
				Name = name,
				AuthorId = _currentContext.UserId,
				BudgetId = budgetId
			};

			using (SqlConnection connection = _connectionAccessor())
			{
				int id = await connection.ExecuteScalarAsync<int>(Queries.Plans.Insert, plan);
				plan.Id = id;

				return plan;
			}
		}

		public async Task RemoveAsync(Guid budgetId, int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Plans.Delete, new { Id = planId, BudgetId = budgetId });
			}
		}
	}
}