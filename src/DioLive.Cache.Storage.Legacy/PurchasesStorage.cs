using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class PurchasesStorage : IPurchasesStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public PurchasesStorage(ApplicationDbContext db,
		                        ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Purchase> GetAsync(Guid id)
		{
			return _db.Purchase
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.Single(p => p.Id == id);
		}

		public async Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, string? filter)
		{
			Func<Purchase, bool>? filterFunc = filter is null
				? null
				: new Func<Purchase, bool>(p => p.Name.Contains(filter));

			return await FindAsync(budgetId, filterFunc);
		}

		public async Task<IReadOnlyCollection<Purchase>> GetForStatAsync(Guid budgetId, DateTime dateFrom, DateTime dateTo)
		{
			return await FindAsync(budgetId, p => p.Cost > 0 && p.Date >= dateFrom && p.Date < dateTo);
		}

		public async Task<Guid> AddAsync(Guid budgetId, string name, int categoryId, DateTime date, int cost, string? shop, string? comments)
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

		public async Task UpdateAsync(Guid id, int categoryId, DateTime date, string name, int cost, string? shop, string? comments)
		{
			var purchase = (Models.Purchase) await GetAsync(id);

			purchase.CategoryId = categoryId;
			purchase.Date = date;
			purchase.Name = name;
			purchase.Cost = cost;
			purchase.Shop = shop;
			purchase.Comments = comments;
			purchase.LastEditorId = _currentContext.UserId;

			await _db.SaveChangesAsync();
		}

		public async Task RemoveAsync(Guid id)
		{
			var purchase = (Models.Purchase) await GetAsync(id);

			_db.Purchase.Remove(purchase);

			await _db.SaveChangesAsync();
		}

		public async Task<IReadOnlyCollection<string>> GetShopsAsync(Guid budgetId)
		{
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
			return _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Shop != null)
				.Select(p => p.Shop)
				.Distinct()
				.OrderBy(s => s)
				.ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
		}

		public async Task<IReadOnlyCollection<string>> GetNamesAsync(Guid budgetId, string filter)
		{
			return _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Name.Contains(filter))
				.Select(p => p.Name)
				.Distinct()
				.OrderBy(n => n)
				.ToList();
		}

		private async Task<IReadOnlyCollection<Purchase>> FindAsync(Guid budgetId, Func<Purchase, bool>? filter)
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

			return purchases
				.ToList()
				.AsReadOnly();
		}
	}
}