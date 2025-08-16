using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets;

[Authenticated]
public class CreateJob(
    string name,
    string currencyId
) : Job<Guid>
{
    protected override async Task<Guid> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        Guid budgetId = await storageCollection.Budgets.AddAsync(name, currencyId);
        await storageCollection.Categories.InitializeCategoriesAsync(budgetId);

        return budgetId;
    }
}