using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class OptionsStorage : IOptionsStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public OptionsStorage(Func<SqlConnection> connectionAccessor,
		                      ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<Options> GetAsync()
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				Options options = await connection.QuerySingleOrDefaultAsync<Options>(Queries.Options.Select, new { _currentContext.UserId });
				return options ?? CreateDefaultOptions();
			}
		}

		public async Task SetAsync(int? purchaseGrouping, bool? showPlanList)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				Options options = await connection.QuerySingleOrDefaultAsync<Options>(Queries.Options.Select, new { _currentContext.UserId });

				string sqlQuery;
				if (options is null)
				{
					options = CreateDefaultOptions();
					sqlQuery = Queries.Options.Insert;
				}
				else
				{
					sqlQuery = Queries.Options.Update;
				}

				if (purchaseGrouping.HasValue)
				{
					options.PurchaseGrouping = purchaseGrouping.Value;
				}

				if (showPlanList.HasValue)
				{
					options.ShowPlanList = showPlanList.Value;
				}

				await connection.ExecuteAsync(sqlQuery, options);
			}
		}

		private Options CreateDefaultOptions()
		{
			return new Options
			{
				UserId = _currentContext.UserId,
				PurchaseGrouping = 2,
				ShowPlanList = true
			};
		}
	}
}