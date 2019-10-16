using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class OptionsStorage : IOptionsStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public OptionsStorage(ApplicationDbContext db,
		                      ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Options?> GetAsync()
		{
			return _db.Set<Options>()
				.FirstOrDefault(o => o.UserId == _currentContext.UserId);
		}

		public async Task UpdateAsync(int? purchaseGrouping, bool? showPlanList)
		{
			Options options = _db.Set<Options>()
				.Single(o => o.UserId == _currentContext.UserId);

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

		public async Task CreateAsync(Options options)
		{
			_db.Add(options);

			await _db.SaveChangesAsync();
		}
	}
}