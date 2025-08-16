using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases;

[Authenticated]
[HasRights(ShareAccess.Purchases)]
public class CreateJob(
    string name,
    int categoryId,
    DateTime date,
    decimal cost,
    string? shop,
    string? comments,
    int? planId
) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Purchases.AddAsync(
            CurrentBudget,
            name,
            categoryId,
            date,
            cost,
            shop,
            comments
        );

        if (planId.HasValue)
        {
            await storageCollection.Plans.BuyAsync(planId.Value);
        }
    }
}