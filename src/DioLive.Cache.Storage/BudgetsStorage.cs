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
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public BudgetsStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<(Result, Budget)> GetAsync(Guid id, ShareAccess shareAccess)
		{
			Budget budget = await _db.Budget
				.Include(b => b.Shares)
				.SingleOrDefaultAsync(b => b.Id == id);

			if (budget == null)
			{
				return (Result.NotFound, default);
			}

			if (!budget.HasRights(_currentContext.UserId, shareAccess))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, budget);
		}

		public async Task<Budget> GetDetailsAsync(Guid id)
		{
			return await _db.Budget
				.Include(b => b.Author)
				.Include(b => b.Plans)
				.SingleOrDefaultAsync(b => b.Id == id);
		}

		public async Task<Budget> GetForBudgetSharingComponentAsync(Guid id)
		{
			return await _db.Budget
				.Include(b => b.Author)
				.Include(b => b.Shares).ThenInclude(s => s.User)
				.SingleOrDefaultAsync(b => b.Id == id);
		}

		public async Task<List<Budget>> GetForUserBudgetsComponentAsync()
		{
			string userId = _currentContext.UserId;

			return await _db.Budget
				.Include(b => b.Shares)
				.Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId))
				.ToListAsync();
		}

		public async Task<Guid> AddAsync(string name)
		{
			var budget = new Budget
			{
				Id = Guid.NewGuid(),
				Name = name,
				AuthorId = _currentContext.UserId,
				Version = 2
			};

			await _db.AddAsync(budget);
			await _db.SaveChangesAsync();

			return budget.Id;
		}

		public async Task<Result> RenameAsync(Guid id, string name)
		{
			(Result result, Budget budget) = await GetAsync(id, ShareAccess.Manage);

			if (result != Result.Success)
			{
				return result;
			}

			budget.Name = name;

			return await SaveChangesAsync(id);
		}

		public async Task<Result> RemoveAsync(Guid id)
		{
			(Result result, Budget budget) = await GetAsync(id, ShareAccess.Delete);

			if (result != Result.Success)
			{
				return result;
			}

			_db.Budget.Remove(budget);

			return await SaveChangesAsync(id);
		}

		public async Task<Result> ShareAsync(Guid id, string userId, ShareAccess access)
		{
			(Result result, Budget budget) = await GetAsync(id, ShareAccess.Manage);

			if (result != Result.Success)
			{
				return result;
			}

			Share share = budget.Shares.SingleOrDefault(s => s.UserId == userId);

			if (share != null)
			{
				share.Access = access;
			}
			else
			{
				share = new Share
				{
					BudgetId = id,
					UserId = userId,
					Access = access
				};
				await _db.AddAsync(share);
			}

			return await SaveChangesAsync(id);
		}

		public async Task<(Result, Budget)> OpenAsync(Guid id)
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

			if (!budget.HasRights(_currentContext.UserId, ShareAccess.Categories))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, budget);
		}

		public async Task<Result> MigrateAsync(Guid id)
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

			return await SaveChangesAsync(id);
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
				return await _db.Budget.AnyAsync(b => b.Id == id) ? Result.Error : Result.NotFound;
			}
		}
	}
}