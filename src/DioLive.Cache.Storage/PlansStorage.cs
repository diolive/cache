using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Models.Data;
using DioLive.Cache.Storage.Contracts;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage
{
	public class PlansStorage : IPlansStorage
	{
		private readonly ApplicationDbContext _db;

		public PlansStorage(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Plan> FindAsync(Guid budgetId, int planId)
		{
			Budget budget = await _db.Budget
				.Include(b => b.Plans)
				.SingleAsync(b => b.Id == budgetId);

			return budget.Plans
				.SingleOrDefault(p => p.Id == planId);
		}

		public async Task BuyAsync(Guid budgetId, int planId, string buyerId)
		{
			Plan plan = await FindAsync(budgetId, planId);

			if (plan != null)
			{
				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = buyerId;
				await _db.SaveChangesAsync();
			}
		}

		public async Task<Plan> AddAsync(Guid budgetId, string name, string authorId)
		{
			Budget budget = await _db.Budget
				.Include(b => b.Plans)
				.SingleOrDefaultAsync(b => b.Id == budgetId);

			var plan = new Plan
			{
				Name = name,
				AuthorId = authorId
			};
			budget.Plans.Add(plan);
			await _db.SaveChangesAsync();

			return plan;
		}

		public async Task RemoveAsync(Guid budgetId, int planId)
		{
			Plan plan = _db.Budget
				.Include(b => b.Plans)
				.Single(b => b.Id == budgetId)
				.Plans
				.Single(p => p.Id == planId);

			_db.Set<Plan>().Remove(plan);
			await _db.SaveChangesAsync();
		}
	}
}