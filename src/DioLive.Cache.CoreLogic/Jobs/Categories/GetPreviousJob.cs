using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories;

[Authenticated]
[HasAnyRights]
public class GetPreviousJob(string purchaseName) : Job<int?>
{
    protected override async Task<int?> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.GetLatestAsync(CurrentBudget, purchaseName);
    }
}