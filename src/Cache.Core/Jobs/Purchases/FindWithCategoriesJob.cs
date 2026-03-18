using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

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