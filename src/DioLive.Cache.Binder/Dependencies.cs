using DioLive.Cache.Auth;
using DioLive.Cache.Auth.Legacy;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.SqlServer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.Cache.Binder
{
	public static class Dependencies
	{
		public static void BindCacheDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			// Authentication
			services.AddLegacyAuth(configuration.GetConnectionString("Auth"));
			services.AddScoped<AppUserManager>();

			// Storage
			services.AddSingleton(typeof(IConnectionInfo), new ConnectionInfo(configuration.GetConnectionString("Data")));
			services.AddScoped<IBudgetsStorage, BudgetsStorage>();
			services.AddScoped<ICategoriesStorage, CategoriesStorage>();
			services.AddScoped<ICurrenciesStorage, CurrenciesStorage>();
			services.AddScoped<IOptionsStorage, OptionsStorage>();
			services.AddScoped<IPlansStorage, PlansStorage>();
			services.AddScoped<IPurchasesStorage, PurchasesStorage>();
			services.AddScoped<IUsersStorage, UsersStorage>();
			services.AddScoped<IStorageCollection, StorageCollection>();

			// Permissions
			services.AddScoped<IPermissionsValidator, PermissionsValidator>();

			// Logic
			services.AddScoped<JobSettings>();
			services.AddScoped<IBudgetsLogic, BudgetsLogic>();
			services.AddScoped<ICategoriesLogic, CategoriesLogic>();
			services.AddScoped<IChartsLogic, ChartsLogic>();
			services.AddScoped<ICurrenciesLogic, CurrenciesLogic>();
			services.AddScoped<IOptionsLogic, OptionsLogic>();
			services.AddScoped<IPlansLogic, PlansLogic>();
			services.AddScoped<IPurchasesLogic, PurchasesLogic>();
			services.AddScoped<IUsersLogic, UsersLogic>();
		}
	}
}