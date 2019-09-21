using System;
using System.Data;
using System.Data.SqlClient;

using DioLive.Cache.Storage.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace DioLive.Cache.Storage.SqlServer
{
	public static class ServicesExtension
	{
		public static void AddSqlStorages(this IServiceCollection services, string connectionString)
		{
			services.AddSingleton<Func<IDbConnection>>(() => new SqlConnection(connectionString));
			services.AddTransient<IBudgetsStorage, BudgetsStorage>();
			services.AddTransient<ICategoriesStorage, CategoriesStorage>();
			services.AddTransient<IOptionsStorage, OptionsStorage>();
			services.AddTransient<IPlansStorage, PlansStorage>();
			services.AddTransient<IPurchasesStorage, PurchasesStorage>();
		}
	}
}