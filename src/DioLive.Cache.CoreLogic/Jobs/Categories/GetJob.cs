using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories;

[Authenticated]
[HasAnyRights]
public class GetJob(int categoryId) : Job<Category?>
{
    protected override void CustomValidation()
    {
        AssertCategoryIsInCurrentBudget(categoryId);
    }

    protected override async Task<Category?> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Categories.GetAsync(categoryId);
    }
}