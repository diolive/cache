using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Options;

[Authenticated]
public class UpdateJob(int? purchaseGrouping, bool? showPlanList) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollections = Settings.StorageCollection;

        await storageCollections.Options.UpdateAsync(purchaseGrouping, showPlanList);
    }
}