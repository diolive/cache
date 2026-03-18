using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

[Authenticated]
[HasAnyRights]
public class GetJob(Guid purchaseId) : Job<Purchase?>
{
    protected override async Task<Purchase?> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Purchases.GetAsync(purchaseId);
    }
}