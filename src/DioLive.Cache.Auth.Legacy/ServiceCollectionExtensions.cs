using DioLive.Cache.Auth.Legacy.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.Cache.Auth.Legacy
{
	public static class ServiceCollectionExtensions
	{
		public static void AddLegacyAuth(this IServiceCollection services,
		                                 string connectionString)
		{
			// Add framework services.
			services.AddDbContext<AuthDbContext>(options => { options.UseSqlServer(connectionString); });

			services.AddIdentity<IdentityUser, IdentityRole>(options =>
				{
					options.Password.RequiredLength = 6;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
				})
				.AddEntityFrameworkStores<AuthDbContext>()
				.AddDefaultTokenProviders();
		}
	}
}