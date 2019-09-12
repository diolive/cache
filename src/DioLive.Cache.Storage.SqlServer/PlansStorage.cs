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

		public async Task<Plan> FindAsync(int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = CurrentBudgetId });
			}
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync()
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<Plan>(Queries.Plans.SelectAll, new { BudgetId = CurrentBudgetId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task BuyAsync(int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				Plan plan = await connection.QuerySingleOrDefaultAsync<Plan>(Queries.Plans.Select, new { Id = planId, BudgetId = CurrentBudgetId });

				if (plan is null)
				{
					return;
				}

				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = _currentContext.UserId;

				await connection.ExecuteAsync(Queries.Plans.Buy, plan);
			}
		}

		public async Task<Plan> AddAsync(string name)
		{
			var plan = new Plan
			{
				Name = name,
				AuthorId = _currentContext.UserId,
				BudgetId = CurrentBudgetId
			};

			using (SqlConnection connection = _connectionAccessor())
			{
				int id = await connection.ExecuteScalarAsync<int>(Queries.Plans.Insert, plan);
				plan.Id = id;

				return plan;
			}
		}

		public async Task RemoveAsync(int planId)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Plans.Delete, new { Id = planId, BudgetId = CurrentBudgetId });
			}
		}

		private Guid CurrentBudgetId => _currentContext.BudgetId.Value;
	}
}