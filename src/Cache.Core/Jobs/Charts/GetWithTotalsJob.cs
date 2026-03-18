using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Charts;

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