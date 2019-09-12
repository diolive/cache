using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.AspNetCore.Identity;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class IdentityUsersStorage : IUsersStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public IdentityUsersStorage(ApplicationDbContext db,
		                            ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<string> GetUserNameAsync(string id)
		{
			return (await GetAsync(id)).UserName;
		}

		public async Task<string> FindByUserNameAsync(string userName)
		{
			IdentityUser user = _db.Users.SingleOrDefault(u => u.NormalizedUserName == userName.ToUpperInvariant());
			return user?.Id;
		}

		public async Task<IdentityUser> GetCurrent()
		{
			return await GetAsync(_currentContext.UserId);
		}

		private async Task<IdentityUser> GetAsync(string id)
		{
			return _db.Users.Single(u => u.Id == id);
		}
	}
}