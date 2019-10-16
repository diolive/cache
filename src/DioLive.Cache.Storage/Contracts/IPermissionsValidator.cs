using System;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IPermissionsValidator
	{
		Task<ResultStatus> CheckUserRightsForBudgetAsync(Guid budgetId, string userId, ShareAccess requiredAccess);
		ResultStatus CheckUserRightsForBudget(Guid budgetId, string userId, ShareAccess requiredAccess);
		Task<ResultStatus> CheckUserRightsForCategoryAsync(int categoryId, string userId, ShareAccess requiredAccess);
		ResultStatus CheckUserRightsForCategory(int categoryId, string userId, ShareAccess requiredAccess);
		Task<ResultStatus> CheckUserRightsForPurchaseAsync(Guid purchaseId, string userId, ShareAccess requiredAccess);
		ResultStatus CheckUserRightsForPurchase(Guid purchaseId, string userId, ShareAccess requiredAccess);
		Task<ResultStatus> CheckUserCanRenameBudgetAsync(Guid budgetId, string userId);
		ResultStatus CheckUserCanRenameBudget(Guid budgetId, string userId);
		Task<ResultStatus> CheckUserCanDeleteBudgetAsync(Guid budgetId, string userId);
		ResultStatus CheckUserCanDeleteBudget(Guid budgetId, string userId);
		Task<ResultStatus> CheckUserCanCreateCategoryAsync(Guid budgetId, string userId);
		ResultStatus CheckUserCanCreateCategory(Guid budgetId, string userId);
		Task<ResultStatus> CheckUserCanEditPurchaseAsync(Guid purchaseId, string userId);
		ResultStatus CheckUserCanEditPurchase(Guid purchaseId, string userId);
		Task<ResultStatus> CheckUserCanDeletePurchaseAsync(Guid purchaseId, string userId);
		ResultStatus CheckUserCanDeletePurchase(Guid purchaseId, string userId);
	}
}