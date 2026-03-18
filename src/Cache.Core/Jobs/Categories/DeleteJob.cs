using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

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