using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Domain;

public class StorageCollection(
    IBudgetsStorage budgets,
    ICategoriesStorage categories,
    ICurrenciesStorage currencies,
    IOptionsStorage options,
    IPlansStorage plans,
    IPurchasesStorage purchases,
    IUsersStorage users
) : IStorageCollection
{
    public IBudgetsStorage Budgets { get; } = budgets;
    public ICategoriesStorage Categories { get; } = categories;
    public ICurrenciesStorage Currencies { get; } = currencies;
    public IOptionsStorage Options { get; } = options;
    public IPlansStorage Plans { get; } = plans;
    public IPurchasesStorage Purchases { get; } = purchases;
    public IUsersStorage Users { get; } = users;
}