using DioRed.Cache.Infrastructure.Auth;
using DioRed.Cache.Core;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Repositories;
using DioRed.Cache.Infrastructure.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Cache.Infrastructure;

public static class Dependencies
{
    public static void AddCacheInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Authentication
        services.AddLegacyAuth(configuration.GetConnectionString("Auth")!);
        services.AddScoped<AppUserManager>();

        // Storage
        services.AddSingleton<IConnectionInfo>(new ConnectionInfo(configuration.GetConnectionString("Data")!));
        services.AddScoped<IBudgetsStorage, BudgetsStorage>();
        services.AddScoped<ICategoriesStorage, CategoriesStorage>();
        services.AddScoped<ICurrenciesStorage, CurrenciesStorage>();
        services.AddScoped<IOptionsStorage, OptionsStorage>();
        services.AddScoped<IPlansStorage, PlansStorage>();
        services.AddScoped<IPurchasesStorage, PurchasesStorage>();
        services.AddScoped<IUsersStorage, UsersStorage>();
        services.AddScoped<IStorageCollection, StorageCollection>();
        services.AddSingleton<IAvatarUrlProvider, GravatarHelper>();

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
