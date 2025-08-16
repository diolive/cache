using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories;

[Authenticated]
[HasRights(ShareAccess.Categories)]
public class DeleteJob(int categoryId) : Job
{
    protected override void CustomValidation()
    {
        AssertCategoryIsInCurrentBudget(categoryId);
    }

    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Categories.DeleteAsync(categoryId);
    }
}