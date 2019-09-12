using System;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class OptionsStorage : StorageBase, IOptionsStorage
	{
		public OptionsStorage(Func<IDbConnection> connectionAccessor,
		                      ICurrentContext currentContext)
			: base(connectionAccessor, currentContext)
		{
		}

		public async Task<Options> GetAsync()
		{
			using (IDbConnection connection = OpenConnection())
			{
				Options options = await connection.QuerySingleOrDefaultAsync<Options>(Queries.Options.Select, new { CurrentUserId });
				return options ?? CreateDefaultOptions();
			}
		}

		public async Task SetAsync(int? purchaseGrouping, bool? showPlanList)
		{
			using (IDbConnection connection = OpenConnection())
			{
				Options options = await connection.QuerySingleOrDefaultAsync<Options>(Queries.Options.Select, new { CurrentUserId });

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
				UserId = CurrentUserId,
				PurchaseGrouping = 2,
				ShowPlanList = true
			};
		}
	}
}