using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

using Category = DioLive.Cache.Storage.Legacy.Models.Category;
using CategoryLocalization = DioLive.Cache.Storage.Legacy.Models.CategoryLocalization;
using Purchase = DioLive.Cache.Storage.Legacy.Models.Purchase;

namespace DioLive.Cache.Storage.Legacy
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
			Models.Budget budget = await _db.Budget
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

		public async Task<IReadOnlyCollection<Budget>> GetForUserBudgetsComponentAsync()
		{
			string userId = _currentContext.UserId;

			return await _db.Budget
				.Include(b => b.Shares)
				.Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId))
				.ToListAsync();
		}

		public async Task<Guid> AddAsync(string name)
		{
			var budget = new Models.Budget
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

			_db.Budget.Remove((Models.Budget)budget);

			return await SaveChangesAsync(id);
		}

		public async Task<Result> ShareAsync(Guid id, string userId, ShareAccess access)
		{
			(Result result, Budget budget) = await GetAsync(id, ShareAccess.Manage);

			if (result != Result.Success)
			{
				return result;
			}

			Share share = ((Models.Budget)budget).Shares.SingleOrDefault(s => s.UserId == userId);

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
			Models.Budget budget = await _db.Budget
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
			Models.Budget budget = await _db.Budget
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

		public async Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId)
		{
			return await _db.Set<Share>()
				.Where(s => s.BudgetId == budgetId)
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
				return await _db.Budget.AnyAsync(b => b.Id == id) ? Result.Error : Result.NotFound;
			}
		}
	}
}