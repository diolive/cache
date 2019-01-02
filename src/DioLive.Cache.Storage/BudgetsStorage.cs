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
	public class BudgetsStorage : IBudgetsStorage
	{
		private readonly ApplicationDbContext _db;

		public BudgetsStorage(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Budget> GetByIdAsync(Guid id)
		{
			return await _db.Budget
				.Include(b => b.Author)
				.Include(b => b.Plans)
				.SingleOrDefaultAsync(b => b.Id == id);
		}

		public async Task<Budget> GetWithSharesAsync(Guid id)
		{
			return await _db.Budget
				.Include(b => b.Shares)
				.SingleOrDefaultAsync(b => b.Id == id);
		}

		public async Task<Budget> GetForBudgetSharingComponentAsync(Guid id)
		{
			return await _db.Budget
				.Include(b => b.Author)
				.Include(b => b.Shares).ThenInclude(s => s.User)
				.SingleOrDefaultAsync(b => b.Id == id);
		}

		public async Task<List<Budget>> GetForUserBudgetsComponentAsync(string userId)
		{
			return await _db.Budget
				.Include(b => b.Shares)
				.Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId))
				.ToListAsync();
		}

		public async Task AddAsync(Budget budget)
		{
			await _db.AddAsync(budget);
			await _db.SaveChangesAsync();
		}

		public async Task<Result> RenameAsync(Guid id, string userId, string name)
		{
			Budget budget = await GetWithSharesAsync(id);

			if (budget == null)
			{
				return Result.NotFound;
			}

			if (!budget.HasRights(userId, ShareAccess.Manage))
			{
				return Result.Forbidden;
			}

			budget.Name = name;

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Budget.AnyAsync(b => b.Id == budget.Id) ? Result.Error : Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<(Result, Budget)> GetForRemoveAsync(Guid id, string userId)
		{
			Budget budget = await GetWithSharesAsync(id);

			if (budget == null)
			{
				return (Result.NotFound, default);
			}

			if (!budget.HasRights(userId, ShareAccess.Delete))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, budget);
		}

		public async Task<Result> RemoveAsync(Guid id, string userId)
		{
			Budget budget = await GetWithSharesAsync(id);

			if (budget == null)
			{
				return Result.NotFound;
			}

			if (!budget.HasRights(userId, ShareAccess.Delete))
			{
				return Result.Forbidden;
			}

			_db.Budget.Remove(budget);

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Budget.AnyAsync(b => b.Id == id) ? Result.Error : Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<Result> ShareAsync(Guid id, string authorId, string targetUserId, ShareAccess access)
		{
			Budget budget = await GetWithSharesAsync(id);
			if (budget == null)
			{
				return Result.NotFound;
			}

			if (!budget.HasRights(authorId, ShareAccess.Manage))
			{
				return Result.Forbidden;
			}

			Share share = budget.Shares.SingleOrDefault(s => s.UserId == targetUserId);

			if (share != null)
			{
				share.Access = access;
			}
			else
			{
				share = new Share
				{
					BudgetId = id,
					UserId = targetUserId,
					Access = access
				};
				await _db.AddAsync(share);
			}

			await _db.SaveChangesAsync();
			return Result.Success;
		}

		public async Task<(Result, Budget)> OpenAsync(Guid id, string userId)
		{
			Budget budget = await _db.Budget
				.Include(b => b.Shares)
				.Include(b => b.Categories).ThenInclude(c => c.Purchases)
				.Include(b => b.Categories).ThenInclude(c => c.Localizations)
				.Include(b => b.Categories).ThenInclude(c => c.Subcategories)
				.SingleOrDefaultAsync(b => b.Id == id);

			if (budget == null)
			{
				return (Result.NotFound, default);
			}

			if (!budget.HasRights(userId, ShareAccess.Categories))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, budget);
		}

		public async Task MigrateAsync(Guid id)
		{
			Budget budget = await _db.Budget
				.Include(b => b.Categories)
				.Include(b => b.Purchases)
				.ThenInclude(p => p.Category)
				.SingleAsync(b => b.Id == id);

			if (budget.Version != 1)
			{
				throw new InvalidOperationException($"Couldn't migrate budget from version {budget.Version}.");
			}

			List<Purchase> purchases = budget.Purchases
				.Where(p => p.Category.OwnerId == null)
				.ToList();

			List<Category> categories = await _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToListAsync();

			foreach (Category cat in categories)
			{
				foreach (Purchase pur in purchases.Where(p => p.CategoryId == cat.Id))
				{
					pur.Category = cat;
				}

				cat.Id = default;
				cat.OwnerId = budget.AuthorId;
				foreach (CategoryLocalization tran in cat.Localizations)
				{
					tran.CategoryId = default;
				}

				budget.Categories.Add(cat);
			}

			budget.Version = 2;

			await _db.SaveChangesAsync();
		}
	}
}