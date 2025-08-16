using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Charts;

[Authenticated]
[HasAnyRights]
public class GetWithTotalsJob(int days) : Job<IReadOnlyCollection<CategoryWithTotals>>
{
    protected override async Task<IReadOnlyCollection<CategoryWithTotals>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.GetWithTotalsAsync(CurrentBudget, days);
    }
}