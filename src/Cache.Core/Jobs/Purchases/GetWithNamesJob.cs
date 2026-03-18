using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

[Authenticated]
[HasAnyRights]
public class GetWithNamesJob(Guid purchaseId) : Job<PurchaseWithNames?>
{
    protected override async Task<PurchaseWithNames?> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        if (await storageCollection.Purchases.GetAsync(purchaseId) is not { } purchase)
        {
            return null;
        }

        string authorName = await storageCollection.Users.GetNameByIdAsync(purchase.AuthorId)
            ?? purchase.AuthorId;

        string? lastEditorName = purchase.LastEditorId is null
            ? null
            : await storageCollection.Users.GetNameByIdAsync(purchase.LastEditorId);

        return new PurchaseWithNames
        {
            Purchase = purchase,
            AuthorName = authorName,
            LastEditorName = lastEditorName
        };
    }
}