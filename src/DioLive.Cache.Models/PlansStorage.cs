using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

using Budget = DioLive.Cache.Storage.Legacy.Models.Budget;

namespace DioLive.Cache.Storage.Legacy
{
	public class PlansStorage : IPlansStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public PlansStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Plan> FindAsync(Guid budgetId, int planId)
		{
			return await _db.Set<Models.Plan>()
				.FirstOrDefaultAsync(p => p.Id == planId && p.BudgetId == budgetId);
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId)
		{
			return await _db.Set<Models.Plan>()
				.Where(p => p.BudgetId == budgetId)
				.ToListAsync();
		}

		public async Task BuyAsync(Guid budgetId, int planId)
		{
			Plan plan = await FindAsync(budgetId, planId);

			if (plan != null)
			{
				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = _currentContext.UserId;
				await _db.SaveChangesAsync();
			}
		}

		public async Task<Plan> AddAsync(Guid budgetId, string name)
		{
			var plan = new Models.Plan
			{
				Name = name,
				AuthorId = _currentContext.UserId,
				BudgetId =  budgetId
			};

			await _db.AddAsync(plan);
			await _db.SaveChangesAsync();

			return plan;
		}

		public async Task RemoveAsync(Guid budgetId, int planId)
		{
			Plan plan = await _db.Set<Plan>()
				.FirstAsync(p => p.Id == planId && p.BudgetId == budgetId);

			_db.Set<Plan>().Remove(plan);
			await _db.SaveChangesAsync();
		}
	}
}