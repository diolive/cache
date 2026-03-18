using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Purchases;

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