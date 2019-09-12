using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	internal static class PermissionsValidator
	{
		internal static async Task<Result> CheckUserRightsForBudget(Guid budgetId, string userId, ShareAccess requiredAccess, SqlConnection connection)
		{
			return await connection.ExecuteScalarAsync<Result>(Queries.Budgets.CheckRights, new { BudgetId = budgetId, UserId = userId, Access = requiredAccess });
		}

		internal static async Task<Result> CheckUserRightsForCategory(int categoryId, string userId, ShareAccess requiredAccess, SqlConnection connection)
		{
			return await connection.ExecuteScalarAsync<Result>(Queries.Categories.CheckRights, new { CategoryId = categoryId, UserId = userId, Access = requiredAccess });
		}

		internal static async Task<Result> CheckUserRightsForPurchase(Guid purchaseId, string userId, ShareAccess requiredAccess, SqlConnection connection)
		{
			return await connection.ExecuteScalarAsync<Result>(Queries.Purchases.CheckRights, new { PurchaseId = purchaseId, UserId = userId, Access = requiredAccess });
		}
	}
}