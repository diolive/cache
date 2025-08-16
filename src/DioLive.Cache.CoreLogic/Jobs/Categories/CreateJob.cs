using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories;

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