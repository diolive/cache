using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

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