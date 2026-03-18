using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

[Authenticated]
[HasAnyRights]
public class GetShopsJob : Job<IReadOnlyCollection<string>>
{
    protected override async Task<IReadOnlyCollection<string>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Purchases.GetShopsAsync(CurrentBudget);
    }
}