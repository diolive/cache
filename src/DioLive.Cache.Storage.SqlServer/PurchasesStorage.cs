using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer;

public class PurchasesStorage(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : StorageBase(connectionInfo, currentContext), IPurchasesStorage
{
    public async Task<Purchase?> GetAsync(Guid id)
    {
        return await Connection.QuerySingleOrDefaultAsync<Purchase>(
            Queries.Purchases.Select,
            new
            {
                Id = id
            }
        );
    }

    public async Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, string? filter)
    {
        string nameFilter = string.IsNullOrEmpty(filter)
            ? "%"
            : $"%{filter}%";

        return
        [
            .. await Connection.QueryAsync<Purchase>(
                Queries.Purchases.SelectAll,
                new
                {
                    BudgetId = budgetId,
                    NameFilter = nameFilter
                }
            )
        ];
    }

    public async Task<IReadOnlyCollection<Purchase>> GetForStatAsync(
        Guid budgetId,
        DateTime dateFrom,
        DateTime dateTo
    )
    {
        return
        [
            .. await Connection.QueryAsync<Purchase>(
                Queries.Purchases.SelectForStat,
                new
                {
                    BudgetId = budgetId,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                }
            )
        ];
    }

    public async Task<Guid> AddAsync(
        Guid budgetId,
        string name,
        int categoryId,
        DateTime date,
        decimal cost,
        string? shop,
        string? comments
    )
    {
        Guid purchaseId = Guid.NewGuid();

        var purchase = new Purchase
        {
            Id = purchaseId,
            Name = name,
            CategoryId = categoryId,
            CreateDate = DateTime.UtcNow,
            Date = date,
            Cost = cost,
            Shop = shop,
            Comments = comments,
            AuthorId = CurrentUserId,
            BudgetId = budgetId
        };

        await Connection.ExecuteAsync(Queries.Purchases.Insert, purchase);

        return purchaseId;
    }

    public async Task UpdateAsync(
        Guid id,
        int categoryId,
        DateTime date,
        string name,
        decimal cost,
        string? shop,
        string? comments
    )
    {
        var purchase = new Purchase
        {
            Id = id,
            CategoryId = categoryId,
            Date = date,
            Name = name,
            Cost = cost,
            Shop = shop,
            Comments = comments,
            LastEditorId = CurrentUserId
        };

        await Connection.ExecuteAsync(Queries.Purchases.Update, purchase);
    }

    public async Task RemoveAsync(Guid id)
    {
        await Connection.ExecuteAsync(
            Queries.Purchases.Delete,
            new
            {
                Id = id
            }
        );
    }

    public async Task<IReadOnlyCollection<string>> GetShopsAsync(Guid budgetId)
    {
        return
        [
            .. await Connection.QueryAsync<string>(
                Queries.Purchases.GetShops,
                new
                {
                    BudgetId = budgetId
                }
            )
        ];
    }

    public async Task<IReadOnlyCollection<string>> GetNamesAsync(Guid budgetId, string filter)
    {
        string nameFilter = string.IsNullOrEmpty(filter)
            ? "%"
            : $"%{filter}%";

        return
        [
            .. await Connection.QueryAsync<string>(
                Queries.Purchases.GetNames,
                new
                {
                    BudgetId = budgetId,
                    NameFilter = nameFilter
                }
            )
        ];
    }
}