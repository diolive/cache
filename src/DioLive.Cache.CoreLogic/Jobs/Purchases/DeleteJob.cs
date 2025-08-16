using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases;

[Authenticated]
[HasRights(ShareAccess.Purchases)]
public class DeleteJob(Guid purchaseId) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Purchases.RemoveAsync(purchaseId);
    }
}