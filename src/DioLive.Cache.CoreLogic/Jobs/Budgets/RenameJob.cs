using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets;

[Authenticated]
[HasRights(ShareAccess.Manage)]
public class RenameJob(string newName) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        await storageCollection.Budgets.RenameAsync(CurrentBudget, newName);
    }
}