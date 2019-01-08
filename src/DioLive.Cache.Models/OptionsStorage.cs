﻿using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage.Legacy
{
	public class OptionsStorage : IOptionsStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public OptionsStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Options> GetAsync()
		{
			return await _db.Set<Models.Options>()
				.SingleOrDefaultAsync(o => o.UserId == _currentContext.UserId);
		}

		public async Task UpdateAsync(int? purchaseGrouping, bool? showPlanList)
		{
			Options options = await GetAsync();
			bool exists = options != null;

			if (!exists)
			{
				options = new Models.Options
				{
					UserId = _currentContext.UserId,
					PurchaseGrouping = 2,
					ShowPlanList = true
				};
				await _db.AddAsync(options);
			}

			if (purchaseGrouping.HasValue)
			{
				options.PurchaseGrouping = purchaseGrouping.Value;
			}

			if (showPlanList.HasValue)
			{
				options.ShowPlanList = showPlanList.Value;
			}

			await _db.SaveChangesAsync();
		}
	}
}