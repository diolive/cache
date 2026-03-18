using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
[HasRights(ShareAccess.Delete)]
public class DeleteJob : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Budgets.DeleteAsync(CurrentBudget);
    }
}