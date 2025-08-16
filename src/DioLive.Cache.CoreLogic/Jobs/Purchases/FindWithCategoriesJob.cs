using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases;

[Authenticated]
[HasAnyRights]
public class FindWithCategoriesJob(
    string? filter
) : Job<IReadOnlyCollection<(Purchase purchase, Category category)>>
{
    protected override async Task<IReadOnlyCollection<(Purchase purchase, Category category)>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        IReadOnlyCollection<Purchase> purchases = await storageCollection.Purchases.FindAsync(CurrentBudget, filter);
        IReadOnlyCollection<Category> allCategories = await storageCollection.Categories.GetAllAsync(CurrentBudget);

        return
        [
            ..purchases.Select(purchase =>
            (
                purchase,
                category: allCategories.Single(c => c.Id == purchase.CategoryId)
            ))
        ];
    }
}