using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Currencies;

[Authenticated]
public class GetAllJob : Job<IReadOnlyCollection<Currency>>
{
    protected override async Task<IReadOnlyCollection<Currency>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Currencies.GetAllAsync();
    }
}