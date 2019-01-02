using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Models.Data;
using DioLive.Cache.Storage.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DioLive.Cache.Storage
{
	public class ApplicationUsersStorage : IApplicationUsersStorage
	{
		private readonly ApplicationDbContext _db;

		public ApplicationUsersStorage(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<ApplicationUser> GetWithOptionsAsync(string id)
		{
			return await _db.Users
				.Include(u => u.Options)
				.SingleAsync(u => u.Id == id);
		}

		public async Task<ApplicationUser> GetByUserNameAsync(string userName)
		{
			return await _db.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());
		}

		public async Task UpdateOptionsAsync(string userId, int? purchaseGrouping, bool? showPlanList)
		{
			ApplicationUser user = await GetWithOptionsAsync(userId);

			if (purchaseGrouping.HasValue)
			{
				user.Options.PurchaseGrouping = purchaseGrouping.Value;
			}

			if (showPlanList.HasValue)
			{
				user.Options.ShowPlanList = showPlanList.Value;
			}

			EntityEntry<Options> entry = _db.Entry(user.Options);
			if (entry.State == EntityState.Detached)
			{
				entry.Entity.UserId = user.Id;
				entry.State = EntityState.Added;
			}

			await _db.SaveChangesAsync();
		}
	}
}