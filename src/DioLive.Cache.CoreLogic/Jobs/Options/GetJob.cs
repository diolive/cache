using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Options;

[Authenticated]
public class GetJob : Job<Common.Entities.Options>
{
    protected override async Task<Common.Entities.Options> ExecuteAsync()
    {
        IStorageCollection storageCollections = Settings.StorageCollection;

        Common.Entities.Options? options = await storageCollections.Options.GetAsync();
        if (options is null)
        {
            options = GetDefaultOptions();
            await storageCollections.Options.CreateAsync(options);
        }

        return options;
    }

    private Common.Entities.Options GetDefaultOptions()
    {
        string userId = CurrentContext.GetUserId()
            ?? throw new ApplicationException("Cannot load current user id");

        return new Common.Entities.Options
        {
            UserId = userId,
            PurchaseGrouping = 2,
            ShowPlanList = true
        };
    }
}