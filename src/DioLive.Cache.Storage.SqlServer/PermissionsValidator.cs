using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

using DioRed.Common;

using Microsoft.Data.SqlClient;

namespace DioLive.Cache.Storage.SqlServer;

public class PermissionsValidator(
    IConnectionInfo connectionInfo
) : IPermissionsValidator, IDisposable
{
    private readonly SqlConnection _connection = new(connectionInfo.ConnectionString);

    public async Task<Result> CheckUserRightsForBudgetAsync(
        Guid budgetId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        if (userId is null)
        {
            return Result.Fail();
        }

        int errorCode = await _connection.QueryFirstAsync<int>(
            Queries.Budgets.CheckRights,
            new
            {
                BudgetId = budgetId,
                UserId = userId,
                Access = requiredAccess
            }
        );

        return GetResult(errorCode);
    }

    public Result CheckUserRightsForBudget(
        Guid budgetId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        return CheckUserRightsForBudgetAsync(
            budgetId,
            userId,
            requiredAccess
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserRightsForCategoryAsync(
        int categoryId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        if (userId is null)
        {
            return Result.Fail();
        }

        int errorCode = await _connection.QueryFirstAsync<int>(
            Queries.Categories.CheckRights,
            new
            {
                CategoryId = categoryId,
                UserId = userId,
                Access = requiredAccess
            }
        );
        
        return GetResult(errorCode);
    }

    public Result CheckUserRightsForCategory(
        int categoryId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        return CheckUserRightsForCategoryAsync(
            categoryId,
            userId,
            requiredAccess
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserRightsForPurchaseAsync(
        Guid purchaseId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        if (userId is null)
        {
            return Result.Fail();
        }

        int errorCode = await _connection.QueryFirstAsync<int>(
            Queries.Purchases.CheckRights,
            new
            {
                PurchaseId = purchaseId,
                UserId = userId,
                Access = requiredAccess
            }
        );

        return GetResult(errorCode);
    }

    public Result CheckUserRightsForPurchase(
        Guid purchaseId,
        string? userId,
        ShareAccess requiredAccess
    )
    {
        return CheckUserRightsForPurchaseAsync(
            purchaseId,
            userId,
            requiredAccess
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserCanRenameBudgetAsync(
        Guid budgetId,
        string? userId
    )
    {
        return await CheckUserRightsForBudgetAsync(
            budgetId,
            userId,
            ShareAccess.Manage
        );
    }

    public Result CheckUserCanRenameBudget(
        Guid budgetId,
        string? userId
    )
    {
        return CheckUserCanRenameBudgetAsync(
            budgetId,
            userId
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserCanDeleteBudgetAsync(
        Guid budgetId,
        string? userId
    )
    {
        return await CheckUserRightsForBudgetAsync(
            budgetId,
            userId,
            ShareAccess.Delete
        );
    }

    public Result CheckUserCanDeleteBudget(
        Guid budgetId,
        string? userId
    )
    {
        return CheckUserCanDeleteBudgetAsync(
            budgetId,
            userId
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserCanCreateCategoryAsync(
        Guid budgetId,
        string? userId
    )
    {
        return await CheckUserRightsForBudgetAsync(
            budgetId,
            userId,
            ShareAccess.Categories
        );
    }

    public Result CheckUserCanCreateCategory(
        Guid budgetId,
        string? userId
    )
    {
        return CheckUserCanCreateCategoryAsync(
            budgetId,
            userId
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserCanEditPurchaseAsync(
        Guid purchaseId,
        string? userId
    )
    {
        return await CheckUserRightsForPurchaseAsync(
            purchaseId,
            userId,
            ShareAccess.Purchases
        );
    }

    public Result CheckUserCanEditPurchase(
        Guid purchaseId,
        string? userId
    )
    {
        return CheckUserCanEditPurchaseAsync(
            purchaseId,
            userId
        ).GetAwaiter().GetResult();
    }

    public async Task<Result> CheckUserCanDeletePurchaseAsync(
        Guid purchaseId,
        string? userId
    )
    {
        return await CheckUserRightsForPurchaseAsync(
            purchaseId,
            userId,
            ShareAccess.Purchases
        );
    }

    public Result CheckUserCanDeletePurchase(
        Guid purchaseId,
        string? userId
    )
    {
        return CheckUserCanDeletePurchaseAsync(
            purchaseId,
            userId
        ).GetAwaiter().GetResult();
    }

    private static Result GetResult(int errorCode)
    {
        return errorCode == 0
            ? Result.Success()
            : Result.Fail(new Error(errorCode));
    }

    #region IDisposable implementation
    private bool _isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _connection?.Dispose();
        }

        _isDisposed = true;
    }

    ~PermissionsValidator()
    {
        Dispose(false);
    }
    #endregion
}