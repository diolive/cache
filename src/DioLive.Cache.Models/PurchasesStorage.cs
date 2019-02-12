using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class PurchasesStorage : IPurchasesStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public PurchasesStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<(Result, Purchase)> GetAsync(Guid id)
		{
			Models.Purchase purchase = _db.Purchase
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.SingleOrDefault(p => p.Id == id);

			if (purchase == null)
			{
				return (Result.NotFound, default);
			}

			if (!purchase.Budget.HasRights(_currentContext.UserId, ShareAccess.Purchases))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, purchase);
		}

		public async Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, Func<Purchase, bool> filter)
		{
			IQueryable<Models.Purchase> purchases = _db.Purchase
				.Where(p => p.BudgetId == budgetId);

			if (filter != null)
			{
				purchases = purchases.Where(p => filter(p));
			}

			purchases = purchases
				.OrderByDescending(p => p.Date)
				.ThenByDescending(p => p.CreateDate);

			return purchases.ToList();
		}

		public async Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string shop, string comments)
		{
			var purchase = new Models.Purchase
			{
				Id = Guid.NewGuid(),
				Name = name,
				CategoryId = categoryId,
				CreateDate = DateTime.UtcNow,
				Date = date,
				Cost = cost,
				Shop = shop,
				Comments = comments,
				AuthorId = _currentContext.UserId,
				BudgetId = budgetId
			};

			_db.Add(purchase);
			_db.SaveChanges();

			return purchase.Id;
		}

		public async Task<Result> UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string shop, string comments)
		{
			(Result result, Purchase purchase) = await GetAsync(id);

			if (result != Result.Success)
			{
				return result;
			}

			purchase.CategoryId = categoryId;
			purchase.Date = date;
			purchase.Name = name;
			purchase.Cost = cost;
			purchase.Shop = shop;
			purchase.Comments = comments;
			purchase.LastEditorId = _currentContext.UserId;

			return await SaveChangesAsync(id);
		}

		public async Task<Result> RemoveAsync(Guid id)
		{
			(Result result, Purchase purchase) = await GetAsync(id);

			if (result != Result.Success)
			{
				return result;
			}

			_db.Purchase.Remove((Models.Purchase)purchase);

			return await SaveChangesAsync(id);
		}

		public async Task<List<string>> GetShopsAsync(Guid budgetId)
		{
			return _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Shop != null)
				.Select(p => p.Shop)
				.Distinct()
				.OrderBy(s => s)
				.ToList();
		}

		public async Task<List<string>> GetNamesAsync(Guid budgetId, string filter)
		{
			return _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Name.Contains(filter))
				.Select(p => p.Name)
				.Distinct()
				.OrderBy(n => n)
				.ToList();
		}

		public async Task<int> CalculateTotalCostAsync(Guid budgetId, int categoryId, int days)
		{
			IQueryable<Purchase> purchases = _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.CategoryId == categoryId && p.Cost > 0);

			if (days > 0)
			{
				DateTime minDate = DateTime.Today.AddDays(-days);
				purchases = purchases.Where(p => p.Date >= minDate);
			}

			return purchases.Sum(p => p.Cost);
		}

		private async Task<Result> SaveChangesAsync(Guid id)
		{
			try
			{
				_db.SaveChanges();
				return Result.Success;
			}
			catch (DbUpdateConcurrencyException)
			{
				return _db.Purchase.Any(p => p.Id == id)
					? Result.Error
					: Result.NotFound;
			}
		}
	}
}