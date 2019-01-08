using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;
using DioLive.Cache.Storage.Legacy.Models;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage.Legacy
{
	public class ApplicationUsersStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly IOptionsStorage _optionsStorage;
		private readonly ApplicationDbContext _db;

		public ApplicationUsersStorage(ApplicationDbContext db,
									   ICurrentContext currentContext,
									   IOptionsStorage optionsStorage)
		{
			_db = db;
			_currentContext = currentContext;
			_optionsStorage = optionsStorage;
		}

		public async Task<ApplicationUser> GetAsync(string id, bool loadOptions = false)
		{
			IQueryable<ApplicationUser> users = _db.Users;
			if (loadOptions)
			{
				users = users.Include(u => u.Options);
			}

			return await users
				.SingleAsync(u => u.Id == id);
		}

		public async Task<ApplicationUser> GetCurrentAsync(bool loadOptions = false)
		{
			return await GetAsync(_currentContext.UserId, loadOptions);
		}

		public async Task<ApplicationUser> GetByUserNameAsync(string userName)
		{
			return await _db.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());
		}
	}
}