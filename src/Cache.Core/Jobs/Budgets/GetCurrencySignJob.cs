using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
[HasAnyRights]
public class GetCurrencySignJob : Job<string>
{
    protected override async Task<string> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;
        string currency = await storageCollection.Budgets.GetCurrencyAsync(CurrentBudget);

        return currency;
    }
}