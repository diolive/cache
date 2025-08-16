using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases;

[Authenticated]
[HasRights(ShareAccess.Purchases)]
public class UpdateJob(
    Guid id,
    string name,
    int categoryId,
    DateTime date,
    decimal cost,
    string? shop,
    string? comments
) : Job
{
    protected override void CustomValidation()
    {
        AssertCategoryIsInCurrentBudget(categoryId);
        AssertPurchaseIsInCurrentBudget(id);
    }

    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Purchases.UpdateAsync(
            id,
            categoryId,
            date,
            name,
            cost,
            shop,
            comments
        );
    }
}