using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

[Authenticated]
[HasAnyRights]
public class GetAllJob : Job<IReadOnlyCollection<Category>>
{
    protected override async Task<IReadOnlyCollection<Category>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.GetAllAsync(CurrentBudget);
    }
}