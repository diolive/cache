using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

#pragma warning disable 1998

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
			return _db.Set<Models.Plan>()
				.FirstOrDefault(p => p.Id == planId && p.BudgetId == budgetId);
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId)
		{
			return _db.Set<Models.Plan>()
				.Where(p => p.BudgetId == budgetId)
				.ToList();
		}

		public async Task BuyAsync(Guid budgetId, int planId)
		{
			Plan plan = await FindAsync(budgetId, planId);

			if (plan != null)
			{
				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = _currentContext.UserId;
				_db.SaveChanges();
			}
		}

		public async Task<Plan> AddAsync(Guid budgetId, string name)
		{
			var plan = new Models.Plan
			{
				Name = name,
				AuthorId = _currentContext.UserId,
				BudgetId = budgetId
			};

			_db.Add(plan);
			_db.SaveChanges();

			return plan;
		}

		public async Task RemoveAsync(Guid budgetId, int planId)
		{
			Plan plan = _db.Set<Plan>()
				.First(p => p.Id == planId && p.BudgetId == budgetId);

			_db.Set<Plan>().Remove(plan);
			_db.SaveChanges();
		}
	}
}