using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;
using DioLive.Cache.Storage.Legacy.Models;

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
			return (await GetAsync(id)).UserName;
		}

		public async Task<string> FindByUserNameAsync(string userName)
		{
			ApplicationUser user = _db.Users.SingleOrDefault(u => u.NormalizedUserName == userName.ToUpperInvariant());
			return user?.Id;
		}

		public async Task<ApplicationUser> GetCurrent()
		{
			return await GetAsync(_currentContext.UserId);
		}

		private async Task<ApplicationUser> GetAsync(string id)
		{
			return _db.Users.Single(u => u.Id == id);
		}
	}
}