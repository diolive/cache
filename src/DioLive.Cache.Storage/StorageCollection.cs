using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage
{
	public class StorageCollection : IStorageCollection
	{
		public StorageCollection(IBudgetsStorage budgets,
		                         ICategoriesStorage categories,
		                         ICurrenciesStorage currencies,
		                         IOptionsStorage options,
		                         IPlansStorage plans,
		                         IPurchasesStorage purchases,
		                         IUsersStorage users)
		{
			Budgets = budgets;
			Categories = categories;
			Currencies = currencies;
			Options = options;
			Plans = plans;
			Purchases = purchases;
			Users = users;
		}

		public IBudgetsStorage Budgets { get; }
		public ICategoriesStorage Categories { get; }
		public ICurrenciesStorage Currencies { get; }
		public IOptionsStorage Options { get; }
		public IPlansStorage Plans { get; }
		public IPurchasesStorage Purchases { get; }
		public IUsersStorage Users { get; }
	}
}