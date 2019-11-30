namespace DioLive.Cache.Storage.Contracts
{
	public interface IStorageCollection
	{
		IBudgetsStorage Budgets { get; }
		ICategoriesStorage Categories { get; }
		ICurrenciesStorage Currencies { get; }
		IOptionsStorage Options { get; }
		IPlansStorage Plans { get; }
		IPurchasesStorage Purchases { get; }
		IUsersStorage Users { get; }
	}
}