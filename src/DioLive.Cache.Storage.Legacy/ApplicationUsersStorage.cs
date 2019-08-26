using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;
using DioLive.Cache.Storage.Legacy.Models;

using Microsoft.EntityFrameworkCore;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class ApplicationUsersStorage : IUsersStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public ApplicationUsersStorage(ApplicationDbContext db,
		                               ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<string> GetUserNameAsync(string id)
		{
			return (await GetAsync(id, false)).UserName;
		}

		public async Task<string> FindByUserNameAsync(string userName)
		{
			ApplicationUser user = _db.Users.SingleOrDefault(u => u.NormalizedUserName == userName.ToUpperInvariant());
			return user?.Id;
		}

		public async Task<ApplicationUser> GetCurrentUserWithOptionsAsync()
		{
			return await GetAsync(_currentContext.UserId, true);
		}

		private async Task<ApplicationUser> GetAsync(string id, bool loadOptions)
		{
			IQueryable<ApplicationUser> users = _db.Users;
			if (loadOptions)
			{
				users = users.Include(u => u.Options);
			}

			return users
				.Single(u => u.Id == id);
		}
	}
}