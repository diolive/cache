using Dapper;

using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Infrastructure.Data;

public class OptionsStorage(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : StorageBase(connectionInfo, currentContext), IOptionsStorage
{
    public async Task<Options?> GetAsync()
    {
        return await Connection.QuerySingleOrDefaultAsync<Options>(
            Queries.Options.Select,
            new
            {
                UserId = CurrentUserId
            }
        );
    }

    public async Task UpdateAsync(int? purchaseGrouping, bool? showPlanList)
    {
        Options options = await Connection.QuerySingleAsync<Options>(
            Queries.Options.Select,
            new
            {
                UserId = CurrentUserId
            }
        );

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