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
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public ApplicationUsersStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<ApplicationUser> GetWithOptionsAsync()
		{
			return await _db.Users
				.Include(u => u.Options)
				.SingleAsync(u => u.Id == _currentContext.UserId);
		}

		public async Task<ApplicationUser> GetByUserNameAsync(string userName)
		{
			return await _db.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());
		}

		public async Task UpdateOptionsAsync(int? purchaseGrouping, bool? showPlanList)
		{
			ApplicationUser user = await GetWithOptionsAsync();

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