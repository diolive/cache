using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
[HasAnyRights]
public class GetNameJob : Job<string>
{
    protected override async Task<string> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;
        Budget budget = await storageCollection.Budgets.GetAsync(CurrentBudget)
            ?? throw new ApplicationException("Cannot load current budget");

        return budget.Name;
    }
}