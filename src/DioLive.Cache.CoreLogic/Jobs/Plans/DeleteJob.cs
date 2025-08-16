using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans;

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