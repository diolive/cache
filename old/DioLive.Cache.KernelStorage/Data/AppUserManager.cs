using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.KernelStorage.Data
{
	public class AppUserManager : UserManager<IdentityUser>
	{
		public AppUserManager(IUserStore<IdentityUser> store)
			: base(
				store,
				Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
				new PasswordHasher<IdentityUser>(),
				new IUserValidator<IdentityUser>[] { new UserValidator<IdentityUser>() },
				new IPasswordValidator<IdentityUser>[] { new PasswordValidator<IdentityUser>() },
				new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(),
				new ServiceCollection().BuildServiceProvider(),
				NullLogger<UserManager<IdentityUser>>.Instance)
		{
		}

		public static AppUserManager Instance { get; } = Create();

		public static AppUserManager Create()
		{
			return new AppUserManager(new AppUserStore(new AppDbContext()));
		}

		public async Task<ClaimsIdentity?> GetIdentityAsync(string username, string password)
		{
			IdentityUser? user = await FindByNameAsync(username);
			if (user is null || !await CheckPasswordAsync(user, password))
			{
				return null;
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, username)
			};

			IList<string> roles = await GetRolesAsync(user);
			claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

			return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
		}
	}
}
