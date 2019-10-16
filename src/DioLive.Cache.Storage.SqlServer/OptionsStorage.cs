using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class OptionsStorage : StorageBase, IOptionsStorage
	{
		public OptionsStorage(IConnectionInfo connectionInfo,
		                      ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task<Options?> GetAsync()
		{
			return await Connection.QuerySingleOrDefaultAsync<Options>(Queries.Options.Select, new { UserId = CurrentUserId });
		}

		public async Task UpdateAsync(int? purchaseGrouping, bool? showPlanList)
		{
			Options options = await Connection.QuerySingleAsync<Options>(Queries.Options.Select, new { UserId = CurrentUserId });

			if (purchaseGrouping.HasValue)
			{
				options.PurchaseGrouping = purchaseGrouping.Value;
			}

			if (showPlanList.HasValue)
			{
				options.ShowPlanList = showPlanList.Value;
			}

			await Connection.ExecuteAsync(Queries.Options.Update, options);
		}

		public async Task CreateAsync(Options options)
		{
			await Connection.ExecuteAsync(Queries.Options.Insert, options);
		}
	}
}