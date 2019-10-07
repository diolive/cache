using System;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

using Microsoft.Data.SqlClient;

namespace DioLive.Cache.Storage.SqlServer
{
	public class PermissionsValidator : IPermissionsValidator, IDisposable
	{
		private readonly IDbConnection _connection;
		private bool _isDisposed;

		public PermissionsValidator(IConnectionInfo connectionInfo)
		{
			_connection = new SqlConnection(connectionInfo.ConnectionString);
			_connection.Open();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task<ResultStatus> CheckUserRightsForBudgetAsync(Guid budgetId, string userId, ShareAccess requiredAccess)
		{
			return await _connection.ExecuteScalarAsync<ResultStatus>(Queries.Budgets.CheckRights, new { BudgetId = budgetId, UserId = userId, Access = requiredAccess });
		}

		public ResultStatus CheckUserRightsForBudget(Guid budgetId, string userId, ShareAccess requiredAccess)
		{
			return CheckUserRightsForBudgetAsync(budgetId, userId, requiredAccess).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserRightsForCategoryAsync(int categoryId, string userId, ShareAccess requiredAccess)
		{
			return await _connection.ExecuteScalarAsync<ResultStatus>(Queries.Categories.CheckRights, new { CategoryId = categoryId, UserId = userId, Access = requiredAccess });
		}

		public ResultStatus CheckUserRightsForCategory(int categoryId, string userId, ShareAccess requiredAccess)
		{
			return CheckUserRightsForCategoryAsync(categoryId, userId, requiredAccess).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserRightsForPurchaseAsync(Guid purchaseId, string userId, ShareAccess requiredAccess)
		{
			return await _connection.ExecuteScalarAsync<ResultStatus>(Queries.Purchases.CheckRights, new { PurchaseId = purchaseId, UserId = userId, Access = requiredAccess });
		}

		public ResultStatus CheckUserRightsForPurchase(Guid purchaseId, string userId, ShareAccess requiredAccess)
		{
			return CheckUserRightsForPurchaseAsync(purchaseId, userId, requiredAccess).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserCanRenameBudgetAsync(Guid budgetId, string userId)
		{
			return await CheckUserRightsForBudgetAsync(budgetId, userId, ShareAccess.Manage);
		}

		public ResultStatus CheckUserCanRenameBudget(Guid budgetId, string userId)
		{
			return CheckUserCanRenameBudgetAsync(budgetId, userId).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserCanDeleteBudgetAsync(Guid budgetId, string userId)
		{
			return await CheckUserRightsForBudgetAsync(budgetId, userId, ShareAccess.Delete);
		}

		public ResultStatus CheckUserCanDeleteBudget(Guid budgetId, string userId)
		{
			return CheckUserCanDeleteBudgetAsync(budgetId, userId).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserCanCreateCategoryAsync(Guid budgetId, string userId)
		{
			return await CheckUserRightsForBudgetAsync(budgetId, userId, ShareAccess.Categories);
		}

		public ResultStatus CheckUserCanCreateCategory(Guid budgetId, string userId)
		{
			return CheckUserCanCreateCategoryAsync(budgetId, userId).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserCanEditPurchaseAsync(Guid purchaseId, string userId)
		{
			return await CheckUserRightsForPurchaseAsync(purchaseId, userId, ShareAccess.Purchases);
		}

		public ResultStatus CheckUserCanEditPurchase(Guid purchaseId, string userId)
		{
			return CheckUserCanEditPurchaseAsync(purchaseId, userId).GetAwaiter().GetResult();
		}

		public async Task<ResultStatus> CheckUserCanDeletePurchaseAsync(Guid purchaseId, string userId)
		{
			return await CheckUserRightsForPurchaseAsync(purchaseId, userId, ShareAccess.Purchases);
		}

		public ResultStatus CheckUserCanDeletePurchase(Guid purchaseId, string userId)
		{
			return CheckUserCanDeletePurchaseAsync(purchaseId, userId).GetAwaiter().GetResult();
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
	}
}