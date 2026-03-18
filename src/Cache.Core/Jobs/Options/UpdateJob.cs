using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Options;

[Authenticated]
public class UpdateJob(int? purchaseGrouping, bool? showPlanList) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollections = Settings.StorageCollection;

        await storageCollections.Options.UpdateAsync(purchaseGrouping, showPlanList);
    }
}