using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

[Authenticated]
[HasAnyRights]
public class GetNamesJob(string filter) : Job<IReadOnlyCollection<string>>
{
    protected override async Task<IReadOnlyCollection<string>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Purchases.GetNamesAsync(CurrentBudget, filter);
    }
}