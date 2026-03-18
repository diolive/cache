using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Plans;

[Authenticated]
[HasRights(ShareAccess.Purchases)]
public class DeleteJob(int planId) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Plans.RemoveAsync(planId);
    }
}