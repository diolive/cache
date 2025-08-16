using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer;

public class CurrenciesStorage(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : StorageBase(connectionInfo, currentContext), ICurrenciesStorage
{
    public async Task<IReadOnlyCollection<Currency>> GetAllAsync()
    {
        return
        [
            .. await Connection.QueryAsync<Currency>(Queries.Currencies.SelectAll)
        ];
    }
}