using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Core.Exceptions;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

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