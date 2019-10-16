using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.Auth
{
	public class AppUserManager : UserManager<IdentityUser>
	{
		public AppUserManager(IUserStore<IdentityUser> store,
		                      IOptions<IdentityOptions> optionsAccessor,
		                      IPasswordHasher<IdentityUser> passwordHasher,
		                      IEnumerable<IUserValidator<IdentityUser>> userValidators,
		                      IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
		                      ILookupNormalizer keyNormalizer,
		                      IdentityErrorDescriber errors,
		                      IServiceProvider services,
		                      ILogger<UserManager<IdentityUser>> logger)
			: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public async Task<string> GetUserNameByIdAsync(string userId)
		{
			IdentityUser user = await FindByIdAsync(userId);
			return await GetUserNameAsync(user);
		}

		public async Task<string> GetUserIdByNameAsync(string userName)
		{
			IdentityUser user = await FindByNameAsync(userName);
			return await GetUserIdAsync(user);
		}
	}
}