using System.Threading.Tasks;

using Dapper;

using DioLive.BlackMint.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.Persistence.SQLite
{
    public class IdentityStorage : IIdentityStorage
    {
        private readonly SqliteConnection _connection;

        public IdentityStorage(IOptions<DataSettings> dataOptions)
        {
            _connection = ConnectionHelper.GetConnection(dataOptions.Value.ConnectionString);
        }

        public async Task<int> AddNewUser(string displayName)
        {
            var parameters = new { displayName };

            int userId = await _connection.QueryFirstAsync<int>(Queries.User.Add, parameters);

            return userId;
        }

        public async Task<bool> AddNewUserIdentity(string nameIdentity, int userId)
        {
            var parameters = new { nameIdentity, userId };

            return await _connection.ExecuteAsync(Queries.UserIdentity.Add, parameters) > 0;
        }

        public async Task<User> GetUserById(int id)
        {
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<User>(Queries.User.Get, parameters);
        }

        public async Task<UserIdentity> GetUserIdentity(string nameIdentity)
        {
            var parameters = new { nameIdentity };

            return await _connection.QueryFirstOrDefaultAsync<UserIdentity>(Queries.UserIdentity.Get, parameters);
        }
    }
}