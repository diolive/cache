using System.Threading.Tasks;

using Dapper;

using DioLive.BlackMint.Entities;

using Microsoft.Data.Sqlite;

namespace DioLive.BlackMint.Persistence.SQLite
{
    public class IdentityStorage : IIdentityStorage
    {
        private readonly SqliteConnection _connection;

        public IdentityStorage(SqliteConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddNewUser(string displayName)
        {
            string sql = "INSERT INTO `Users` (`DisplayName`) " +
                         "VALUES (@displayName)" +
                         SqlHelper.SelectIdentity;
            var parameters = new { displayName };

            int userId = await _connection.QueryFirstAsync<int>(sql, parameters);

            return userId;
        }

        public async Task<bool> AddNewUserIdentity(string nameIdentity, int userId)
        {
            string sql = "INSERT INTO `UserIdentities` (`NameIdentity`, `UserId`) " +
                         "VALUES (@nameIdentity, @userId)";
            var parameters = new { nameIdentity, userId };

            return await _connection.ExecuteAsync(sql, parameters) > 0;
        }

        public async Task<User> GetUserById(int id)
        {
            string sql = "SELECT * " +
                         "FROM `Users` " +
                         "WHERE `Id`=@id " +
                         "LIMIT 1";
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }

        public async Task<UserIdentity> GetUserIdentity(string nameIdentity)
        {
            string sql = "SELECT * " +
                         "FROM `UserIdentities` " +
                         "WHERE `NameIdentity`=@nameIdentity " +
                         "LIMIT 1";
            var parameters = new { nameIdentity };

            return await _connection.QueryFirstOrDefaultAsync<UserIdentity>(sql, parameters);
        }
    }
}