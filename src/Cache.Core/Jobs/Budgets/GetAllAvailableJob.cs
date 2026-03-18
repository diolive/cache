using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
public class GetAllAvailableJob : Job<IReadOnlyCollection<Budget>>
{
    protected override async Task<IReadOnlyCollection<Budget>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Budgets.GetAllAvailableAsync();
    }
}