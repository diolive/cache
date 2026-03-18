using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Domain.Repositories;

public interface IPermissionsValidator
{
    Task<Result> CheckUserRightsForBudgetAsync(Guid budgetId, string? userId, ShareAccess requiredAccess);
    Result CheckUserRightsForBudget(Guid budgetId, string? userId, ShareAccess requiredAccess);
    Task<Result> CheckUserRightsForCategoryAsync(int categoryId, string? userId, ShareAccess requiredAccess);
    Result CheckUserRightsForCategory(int categoryId, string? userId, ShareAccess requiredAccess);
    Task<Result> CheckUserRightsForPurchaseAsync(Guid purchaseId, string? userId, ShareAccess requiredAccess);
    Result CheckUserRightsForPurchase(Guid purchaseId, string? userId, ShareAccess requiredAccess);
    Task<Result> CheckUserCanRenameBudgetAsync(Guid budgetId, string? userId);
    Result CheckUserCanRenameBudget(Guid budgetId, string? userId);
    Task<Result> CheckUserCanDeleteBudgetAsync(Guid budgetId, string? userId);
    Result CheckUserCanDeleteBudget(Guid budgetId, string? userId);
    Task<Result> CheckUserCanCreateCategoryAsync(Guid budgetId, string? userId);
    Result CheckUserCanCreateCategory(Guid budgetId, string? userId);
    Task<Result> CheckUserCanEditPurchaseAsync(Guid purchaseId, string? userId);
    Result CheckUserCanEditPurchase(Guid purchaseId, string? userId);
    Task<Result> CheckUserCanDeletePurchaseAsync(Guid purchaseId, string? userId);
    Result CheckUserCanDeletePurchase(Guid purchaseId, string? userId);
}