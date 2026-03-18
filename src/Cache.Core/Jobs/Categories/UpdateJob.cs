using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

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