using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Models.Data;
using DioLive.Cache.Storage.Contracts;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage
{
	public class PurchasesStorage : IPurchasesStorage
	{
		private readonly ApplicationDbContext _db;

		public PurchasesStorage(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<List<Purchase>> FindAsync(Guid budgetId, string filter)
		{
			IQueryable<Purchase> purchases = _db.Purchase.Include(p => p.Category).ThenInclude(c => c.Localizations)
				.Where(p => p.BudgetId == budgetId);

			if (!string.IsNullOrEmpty(filter))
			{
				purchases = purchases.Where(p => p.Name.Contains(filter));
			}

			purchases = purchases
				.OrderByDescending(p => p.Date)
				.ThenByDescending(p => p.CreateDate);

			return await purchases.ToListAsync();
		}

		public async Task<Purchase> GetWithSharesAsync(Guid id)
		{
			return await _db.Purchase
				.Include(p => p.Author)
				.Include(p => p.LastEditor)
				.Include(c => c.Budget)
				.ThenInclude(b => b.Shares)
				.SingleOrDefaultAsync(p => p.Id == id);
		}

		public async Task AddAsync(Purchase purchase)
		{
			await _db.AddAsync(purchase);
			await _db.SaveChangesAsync();
		}

		public async Task<(Result, Purchase)> GetForModificationAsync(Guid id, string userId)
		{
			Purchase purchase = await GetWithSharesAsync(id);
			if (purchase == null)
			{
				return (Result.NotFound, default);
			}

			if (!purchase.Budget.HasRights(userId, ShareAccess.Purchases))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, purchase);
		}

		public async Task<Result> UpdateAsync(Guid id, string userId, int categoryId, DateTime date, string name, int cost, string shop, string comments)
		{
			Purchase purchase = await GetWithSharesAsync(id);

			if (purchase == null)
			{
				return Result.NotFound;
			}

			if (!purchase.Budget.HasRights(userId, ShareAccess.Purchases))
			{
				return Result.Forbidden;
			}

			purchase.CategoryId = categoryId;
			purchase.Date = date;
			purchase.Name = name;
			purchase.Cost = cost;
			purchase.Shop = shop;
			purchase.Comments = comments;
			purchase.LastEditorId = userId;

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Purchase.AnyAsync(p => p.Id == purchase.Id)
					? Result.Error
					: Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<Result> RemoveAsync(Guid id, string userId)
		{
			Purchase purchase = await GetWithSharesAsync(id);

			if (purchase == null)
			{
				return Result.NotFound;
			}

			if (!purchase.Budget.HasRights(userId, ShareAccess.Purchases))
			{
				return Result.Forbidden;
			}

			_db.Purchase.Remove(purchase);

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Purchase.AnyAsync(p => p.Id == id)
					? Result.Error
					: Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<List<string>> GetShopsAsync(Guid budgetId)
		{
			return await _db.Purchase
				.Where(p => p.BudgetId == budgetId)
				.Select(p => p.Shop)
				.Distinct()
				.Except(new string[] { null })
				.OrderBy(s => s)
				.ToListAsync();
		}

		public async Task<List<string>> GetNamesAsync(Guid budgetId, string q)
		{
			return await _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Name.Contains(q))
				.Select(p => p.Name)
				.Distinct()
				.Except(new string[] { null })
				.OrderBy(s => s)
				.ToListAsync();
		}
	}
}