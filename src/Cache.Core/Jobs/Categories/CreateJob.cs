using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

[Authenticated]
[HasRights(ShareAccess.Categories)]
public class CreateJob(string categoryName) : Job<int>
{
    protected override async Task<int> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.AddAsync(categoryName, CurrentBudget);
    }
}