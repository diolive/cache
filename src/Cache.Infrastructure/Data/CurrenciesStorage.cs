using Dapper;

using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Infrastructure.Data;

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