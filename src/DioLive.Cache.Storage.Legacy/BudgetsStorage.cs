using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

using Category = DioLive.Cache.Storage.Legacy.Models.Category;
using Purchase = DioLive.Cache.Storage.Legacy.Models.Purchase;

#pragma warning disable 1998

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

		public async Task<(Result, Budget)> GetAsync(Guid id, ShareAccess requiredAccess)
		{
			Models.Budget budget = _db.Budget
				.Include(b => b.Shares)
				.SingleOrDefault(b => b.Id == id);

			if (budget == null)
			{
				return (Result.NotFound, default);
			}

			if (!budget.HasRights(_currentContext.UserId, requiredAccess))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, budget);
		}

		public async Task<Result> CheckAccessAsync(Guid id, ShareAccess requiredAccess)
		{
			(Result result, _) = await GetAsync(id, requiredAccess);
			return result;
		}

		public async Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync()
		{
			string userId = _currentContext.UserId;

			return _db.Budget
				.Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId))
				.ToList();
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

			_db.Add(budget);
			_db.SaveChanges();

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

			_db.Budget.Remove((Models.Budget) budget);

			return await SaveChangesAsync(id);
		}

		public async Task<Result> ShareAsync(Guid id, string userId, ShareAccess access)
		{
			(Result result, Budget budget) = await GetAsync(id, ShareAccess.Manage);

			if (result != Result.Success)
			{
				return result;
			}

			Share share = ((Models.Budget) budget).Shares.SingleOrDefault(s => s.UserId == userId);

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
				_db.Add(share);
			}

			return await SaveChangesAsync(id);
		}

		public async Task<Result> MigrateAsync(Guid id)
		{
			Models.Budget budget = _db.Budget
				.Include(b => b.Categories)
				.Include(b => b.Purchases)
				.ThenInclude(p => p.Category)
				.Single(b => b.Id == id);

			if (budget.Version != 1)
			{
				throw new InvalidOperationException($"Couldn't migrate budget from version {budget.Version}.");
			}

			List<Purchase> purchases = budget.Purchases
				.Where(p => p.Category.OwnerId == null)
				.ToList();

			List<Category> categories = _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToList();

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
			return _db.Set<Share>()
				.Where(s => s.BudgetId == budgetId)
				.ToList();
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
				return _db.Budget.Any(b => b.Id == id) ? Result.Error : Result.NotFound;
			}
		}
	}
}