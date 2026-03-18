using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DioRed.Cache.Infrastructure.Auth;

public class AppUserManager(
    IUserStore<IdentityUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<IdentityUser> passwordHasher,
    IEnumerable<IUserValidator<IdentityUser>> userValidators,
    IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<IdentityUser>> logger
) : UserManager<IdentityUser>(
    store,
    optionsAccessor,
    passwordHasher,
    userValidators,
    passwordValidators,
    keyNormalizer,
    errors,
    services,
    logger
)
{
    public async Task<string?> GetUserNameByIdAsync(string userId)
    {
        IdentityUser? user = await FindByIdAsync(userId);
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        return await GetUserNameAsync(user);
    }

    public async Task<string?> GetUserIdByNameAsync(string userName)
    {
        IdentityUser? user = await FindByNameAsync(userName);
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        return await GetUserIdAsync(user);
    }
}