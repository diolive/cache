using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories;

[Authenticated]
[HasRights(ShareAccess.Categories)]
public class UpdateJob(
    int categoryId,
    int? parentCategoryId,
    string name,
    string color
) : Job
{
    protected override void CustomValidation()
    {
        AssertCategoryIsInCurrentBudget(categoryId);
        if (parentCategoryId.HasValue)
        {
            AssertCategoryIsInCurrentBudget(parentCategoryId.Value);
        }
    }

    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Categories.UpdateAsync(categoryId, parentCategoryId, name, color);
    }
}