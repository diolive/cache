using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets;

[Authenticated]
[HasRights(ShareAccess.Manage)]
public class ShareJob(
    string targetUserName,
    ShareAccess targetAccess
) : Job
{
    protected override async Task ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        if (await storageCollection.Users.FindIdByNameAsync(targetUserName) is not { } userId)
        {
            throw new NotFoundException($"User with name '{targetUserName}' not found.");
        }

        await storageCollection.Budgets.ShareAsync(CurrentBudget, userId, targetAccess);
    }
}