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
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public PurchasesStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<(Result, Purchase)> GetAsync(Guid id)
		{
			Purchase purchase = await _db.Purchase
				.Include(p => p.Author)
				.Include(p => p.LastEditor)
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.SingleOrDefaultAsync(p => p.Id == id);

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

		public async Task<List<Purchase>> FindAsync(Guid budgetId, string filter)
		{
			IQueryable<Purchase> purchases = _db.Purchase
				.Include(p => p.Category).ThenInclude(c => c.Localizations)
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

		public async Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string shop, string comments)
		{
			var purchase = new Purchase
			{
				Id = Guid.NewGuid(),
				Name = name,
				CategoryId = categoryId,
				CreateDate = DateTime.UtcNow,
				Date = date,
				Cost = cost,
				Shop = shop,
				Comments = comments,
				AuthorId = _currentContext.UserId
			};

			await _db.AddAsync(purchase);
			await _db.SaveChangesAsync();

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

			_db.Purchase.Remove(purchase);

			return await SaveChangesAsync(id);
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

		public async Task<List<string>> GetNamesAsync(Guid budgetId, string filter)
		{
			return await _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Name.Contains(filter))
				.Select(p => p.Name)
				.Distinct()
				.Except(new string[] { null })
				.OrderBy(s => s)
				.ToListAsync();
		}

		private async Task<Result> SaveChangesAsync(Guid id)
		{
			try
			{
				await _db.SaveChangesAsync();
				return Result.Success;
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Purchase.AnyAsync(p => p.Id == id)
					? Result.Error
					: Result.NotFound;
			}
		}
	}
}