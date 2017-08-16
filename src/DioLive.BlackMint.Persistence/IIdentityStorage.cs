using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Persistence
{
    public interface IIdentityStorage
    {
        Task<int> AddNewUser(string displayName);

        Task<bool> AddNewUserIdentity(string nameIdentity, int userId);

        Task<User> GetUserById(int id);

        Task<UserIdentity> GetUserIdentity(string nameIdentity);
    }
}