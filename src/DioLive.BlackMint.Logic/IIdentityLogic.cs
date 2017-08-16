using System;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic
{
    public interface IIdentityLogic
    {
        Task<User> GetUser(int userId);

        Task<User> GetUser(string nameIdentity);

        Task<User> GetOrCreateUser(string nameIdentity, Func<string> displayNameAccessor);

        Task<string> GetUserDisplayName(int userId);
    }
}