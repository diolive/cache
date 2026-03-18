using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

[Authenticated]
[HasAnyRights]
public class GetMostPopularJob : Job<int?>
{
    protected override async Task<int?> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.GetMostPopularIdAsync(CurrentBudget);
    }
}