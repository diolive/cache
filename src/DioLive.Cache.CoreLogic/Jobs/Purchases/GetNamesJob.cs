using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases;

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