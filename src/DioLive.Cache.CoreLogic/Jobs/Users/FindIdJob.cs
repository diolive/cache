using DioLive.Cache.CoreLogic.Attributes;

namespace DioLive.Cache.CoreLogic.Jobs.Users;

[Authenticated]
public class FindIdJob(string name) : Job<string?>
{
    protected override async Task<string?> ExecuteAsync()
    {
        return await Settings.StorageCollection.Users.FindIdByNameAsync(name);
    }
}