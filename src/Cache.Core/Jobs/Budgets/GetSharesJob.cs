using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
[HasRights(ShareAccess.Manage)]
public class GetSharesJob : Job<IReadOnlyCollection<ShareItem>>
{
    protected override async Task<IReadOnlyCollection<ShareItem>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Budgets.GetSharesAsync(CurrentBudget);
    }
}