using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.SqlServer
{
	public class PurchasesStorage : IPurchasesStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public PurchasesStorage(Func<SqlConnection> connectionAccessor,
		                        ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<(Result, Purchase)> GetAsync(Guid id)
		{
			string userId = _currentContext.UserId;

			using (SqlConnection connection = _connectionAccessor())
			{
				Result rights = await PermissionsValidator.CheckUserRightsForPurchase(id, userId, ShareAccess.ReadOnly, connection);

				if (rights != Result.Success)
				{
					return (rights, default);
				}

				Purchase purchase = await connection.QuerySingleOrDefaultAsync<Purchase>(Queries.Purchases.Select, new { Id = id });

				return (Result.Success, purchase);
			}
		}

		public async Task<IReadOnlyCollection<Purchase>> FindAsync(string filter)
		{
			string nameFilter = string.IsNullOrEmpty(filter)
				? "%"
				: $"%{filter}%";

			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<Purchase>(Queries.Purchases.SelectAll, new { BudgetId = CurrentBudgetId, NameFilter = nameFilter }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<IReadOnlyCollection<Purchase>> GetForStatAsync(DateTime dateFrom, DateTime dateTo)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<Purchase>(Queries.Purchases.SelectForStat, new { BudgetId = CurrentBudgetId, DateFrom = dateFrom, DateTo = dateTo }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<Guid> AddAsync(string name, int categoryId, DateTime date, int cost, string shop, string comments)
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
				AuthorId = _currentContext.UserId,
				BudgetId = CurrentBudgetId
			};

			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Purchases.Insert, purchase);
			}

			return purchaseId;
		}

		public async Task<Result> UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string shop, string comments)
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
				LastEditorId = _currentContext.UserId
			};

			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Purchases.Update, purchase);
			}

			return Result.Success;
		}

		public async Task<Result> RemoveAsync(Guid id)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				await connection.ExecuteAsync(Queries.Purchases.Delete, new { Id = id });
			}

			return Result.Success;
		}

		public async Task<IReadOnlyCollection<string>> GetShopsAsync()
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return (await connection.QueryAsync<string>(Queries.Purchases.GetShops, new { BudgetId = CurrentBudgetId }))
					.ToList()
					.AsReadOnly();
			}
		}

		public async Task<IReadOnlyCollection<string>> GetNamesAsync(string filter)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				string nameFilter = string.IsNullOrEmpty(filter)
					? "%"
					: $"%{filter}%";

				return (await connection.QueryAsync<string>(Queries.Purchases.GetNames, new { BudgetId = CurrentBudgetId, NameFilter = nameFilter }))
					.ToList()
					.AsReadOnly();
			}
		}

		private Guid CurrentBudgetId => _currentContext.BudgetId.Value;
	}
}