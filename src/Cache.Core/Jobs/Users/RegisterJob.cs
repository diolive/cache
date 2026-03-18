using DioRed.Cache.Core.Attributes;

namespace DioRed.Cache.Core.Jobs.Users;

[Authenticated]
public class RegisterJob(string userId, string userName) : Job
{
    protected override async Task ExecuteAsync()
    {
        await Settings.StorageCollection.Users.AddAsync(userId, userName);
    }
}