using DioLive.BlackMint.Logic;
using DioLive.BlackMint.Logic.Implementation;
using DioLive.BlackMint.Persistence;
using DioLive.BlackMint.Persistence.SQLite;

using Microsoft.Extensions.DependencyInjection;

namespace DioLive.BlackMint.Depencencies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
        {
            return services.AddSingleton<IIdentityStorage, IdentityStorage>()
                .AddSingleton<IDomainStorage, DomainStorage>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IIdentityLogic, IdentityLogic>()
                .AddSingleton<IDomainLogic, DomainLogic>();
        }
    }
}