using DioLive.Cache.CoreLogic.Attributes;

namespace DioLive.Cache.CoreLogic.Jobs.Users;

[Authenticated]
public class GetNameJob(string id) : Job<string?>
{
    protected override async Task<string?> ExecuteAsync()
    {
        return await Settings.StorageCollection.Users.GetNameByIdAsync(id);
    }
}