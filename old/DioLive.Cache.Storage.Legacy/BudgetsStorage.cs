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
	public class BudgetsStorage : IBudgetsStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public BudgetsStorage(ApplicationDbContext db,
		                      ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Budget> GetAsync(Guid id)
		{
			return await _db.Budget.FindAsync(id);
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

		public async Task RenameAsync(Guid id, string name)
		{
			Models.Budget budget = _db.Budget.Find(id);
			budget.Name = name;
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			Models.Budget budget = _db.Budget.Find(id);
			_db.Budget.Remove(budget);
			await _db.SaveChangesAsync();
		}

		public async Task ShareAsync(Guid id, string userId, ShareAccess access)
		{
			Models.Budget budget = _db.Budget
				.Include(b => b.Shares)
				.Single(b => b.Id == id);

			Share share = budget.Shares.SingleOrDefault(s => s.UserId == userId);

			if (share != null)
			{
				share.Access = access;
			}
			else
			{
				budget.Shares.Add(new Share
				{
					UserId = userId,
					Access = access
				});
			}

			await _db.SaveChangesAsync();
		}

		public async Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId)
		{
			return _db.Set<Share>()
				.Where(s => s.BudgetId == budgetId)
				.ToList();
		}

		public async Task<byte> GetVersionAsync(Guid budgetId)
		{
			return _db.Budget.Single(b => b.Id == budgetId).Version;
		}

		public async Task SetVersionAsync(Guid id, byte version)
		{
			Models.Budget budget = _db.Budget.Single(b => b.Id == id);
			budget.Version = version;
			await _db.SaveChangesAsync();
		}
	}
}