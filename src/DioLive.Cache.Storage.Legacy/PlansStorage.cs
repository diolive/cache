using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
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

		public async Task<Plan> FindAsync(int planId)
		{
			return _db.Set<Plan>()
				.FirstOrDefault(p => p.Id == planId);
		}

		public async Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId)
		{
			return _db.Set<Plan>()
				.Where(p => p.BudgetId == budgetId)
				.ToList();
		}

		public async Task BuyAsync(int planId)
		{
			Plan plan = await FindAsync(planId);

			if (plan != null)
			{
				plan.BuyDate = DateTime.UtcNow;
				plan.BuyerId = _currentContext.UserId;
				_db.SaveChanges();
			}
		}

		public async Task<Plan> AddAsync(string name, Guid budgetId)
		{
			var plan = new Plan
			{
				Name = name,
				AuthorId = _currentContext.UserId,
				BudgetId = budgetId
			};

			_db.Add(plan);
			_db.SaveChanges();

			return plan;
		}

		public async Task RemoveAsync(int planId)
		{
			Plan plan = _db.Set<Plan>()
				.First(p => p.Id == planId);

			_db.Set<Plan>().Remove(plan);
			_db.SaveChanges();
		}
	}
}