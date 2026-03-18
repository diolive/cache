using Dapper;

using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Infrastructure.Data;

public class BudgetsStorage(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : StorageBase(connectionInfo, currentContext), IBudgetsStorage
{
    public async Task<Budget?> GetAsync(Guid id)
    {
        return await Connection.QuerySingleOrDefaultAsync<Budget>(
            Queries.Budgets.Select,
            new
            {
                Id = id
            }
        );
    }

    public async Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync()
    {
        return
        [
            .. await Connection.QueryAsync<Budget>(
                Queries.Budgets.SelectAvailable,
                new
                {
                    UserId = CurrentUserId
                }
            )
        ];
    }

    public async Task<Guid> AddAsync(string name, string currencyId)
    {
        Guid budgetId = Guid.NewGuid();

        string userId = CurrentUserId
            ?? throw new ApplicationException("Cannot load current user id");

        var budget = new Budget
        {
            Id = budgetId,
            Name = name,
            AuthorId = userId,
            CurrencyId = currencyId
        };

        await Connection.ExecuteAsync(Queries.Budgets.Insert, budget);

        return budgetId;
    }

    public async Task RenameAsync(Guid id, string name)
    {
        await Connection.ExecuteAsync(
            Queries.Budgets.Rename,
            new
            {
                Id = id,
                Name = name
            }
        );
    }

    public async Task DeleteAsync(Guid id)
    {
        await Connection.ExecuteAsync(
            Queries.Budgets.Delete,
            new
            {
                Id = id
            }
        );
    }

    public async Task ShareAsync(Guid id, string userId, ShareAccess access)
    {
        var share = new Share
        {
            BudgetId = id,
            UserId = userId,
            Access = access
        };

        await Connection.ExecuteAsync(Queries.Budgets.Share, share);
    }

    public async Task<IReadOnlyCollection<ShareItem>> GetSharesAsync(Guid budgetId)
    {
        return
        [
            .. await Connection.QueryAsync<ShareItem>(
                Queries.Budgets.GetShares,
                new
                {
                    BudgetId = budgetId
                }
            )
        ];
    }

    public async Task<byte> GetVersionAsync(Guid id)
    {
        return await Connection.ExecuteScalarAsync<byte>(
            Queries.Budgets.GetVersion,
            new
            {
                Id = id
            }
        );
    }

    public async Task SetVersionAsync(Guid id, byte version)
    {
        await Connection.ExecuteAsync(
            Queries.Budgets.SetVersion,
            new
            {
                Id = id,
                Version = (byte)2
            }
        );
    }

    public async Task<string> GetCurrencyAsync(Guid id)
    {
        return await Connection.QueryFirstAsync<string>(
            Queries.Budgets.GetCurrency,
            new
            {
                Id = id
            }
        );
    }
}