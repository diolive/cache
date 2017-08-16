using System;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Persistence;

namespace DioLive.BlackMint.Logic.Implementation
{
    public class IdentityLogic : IIdentityLogic
    {
        private readonly IIdentityStorage _identityStorage;
        private readonly ILogger _logger;

        public IdentityLogic(IIdentityStorage identityStorage, ILogger logger)
        {
            _identityStorage = identityStorage;
            _logger = logger;
        }

        public async Task<User> GetUser(int userId)
        {
            Validators.ValidateUserId(userId);

            User user = await _identityStorage.GetUserById(userId);

            return user;
        }

        public async Task<User> GetUser(string nameIdentity)
        {
            Validators.ValidateNameIdentity(nameIdentity);

            UserIdentity userIdentity = await _identityStorage.GetUserIdentity(nameIdentity);
            User user = await _identityStorage.GetUserById(userIdentity.UserId);

            return user;
        }

        public async Task<User> GetOrCreateUser(string nameIdentity, Func<string> displayNameAccessor)
        {
            Validators.ValidateNameIdentity(nameIdentity);

            int userId;

            UserIdentity identity = await _identityStorage.GetUserIdentity(nameIdentity);
            if (identity != null)
            {
                userId = identity.UserId;
            }
            else
            {
                string displayName = displayNameAccessor?.Invoke() ?? nameIdentity;

                userId = await _identityStorage.AddNewUser(displayName);
                await _logger.WriteLog($"Add new user {displayName} (id:{userId})");

                await _identityStorage.AddNewUserIdentity(nameIdentity, userId);
                await _logger.WriteLog($"Add user identity {nameIdentity} for user id:{userId}");
            }

            await _logger.WriteLog($"Login as {nameIdentity} (id:{userId})");

            User user = await _identityStorage.GetUserById(userId);
            return user;
        }

        public async Task<string> GetUserDisplayName(int userId)
        {
            Validators.ValidateUserId(userId);

            User user = await _identityStorage.GetUserById(userId);

            return user.DisplayName;
        }
    }
}