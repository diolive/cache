using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Categories;

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