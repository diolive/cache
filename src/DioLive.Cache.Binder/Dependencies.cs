using DioLive.Cache.Auth;
using DioLive.Cache.Auth.Legacy;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.SqlServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.Cache.Binder
{
	public static class Dependencies
	{
		public static void BindCacheDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			string connectionString = configuration.GetConnectionString("DefaultConnection");

			// Authentication
			services.AddLegacyAuth(connectionString);
			services.AddScoped<AppUserManager>();

			// Storage
			services.AddSingleton(typeof(IConnectionInfo), new ConnectionInfo(connectionString));
			services.AddScoped<IBudgetsStorage, BudgetsStorage>();
			services.AddScoped<ICategoriesStorage, CategoriesStorage>();
			services.AddScoped<IOptionsStorage, OptionsStorage>();
			services.AddScoped<IPlansStorage, PlansStorage>();
			services.AddScoped<IPurchasesStorage, PurchasesStorage>();
			services.AddScoped<IStorageCollection, StorageCollection>();

			// Permissions
			services.AddScoped<IPermissionsValidator, PermissionsValidator>();

			// Logic
			services.AddScoped<JobSettings>();
			services.AddScoped<IBudgetsLogic, BudgetsLogic>();
			services.AddScoped<ICategoriesLogic, CategoriesLogic>();
			services.AddScoped<IChartsLogic, ChartsLogic>();
			services.AddScoped<IOptionsLogic, OptionsLogic>();
			services.AddScoped<IPlansLogic, PlansLogic>();
			services.AddScoped<IPurchasesLogic, PurchasesLogic>();
		}

		public static void UseCacheAuthentication(this IApplicationBuilder app)
		{
			app.UseLegacyAuth();
			app.UseAuthentication();
		}
	}
}