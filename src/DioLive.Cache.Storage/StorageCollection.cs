using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage
{
	public class StorageCollection : IStorageCollection
	{
		public StorageCollection(IBudgetsStorage budgets,
		                         ICategoriesStorage categories,
		                         IOptionsStorage options,
		                         IPlansStorage plans,
		                         IPurchasesStorage purchases)
		{
			Budgets = budgets;
			Categories = categories;
			Options = options;
			Plans = plans;
			Purchases = purchases;
		}

		public IBudgetsStorage Budgets { get; }
		public ICategoriesStorage Categories { get; }
		public IOptionsStorage Options { get; }
		public IPlansStorage Plans { get; }
		public IPurchasesStorage Purchases { get; }
	}
}