using System;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class UsersStorage : StorageBase, IUsersStorage
	{
		public UsersStorage(Func<IDbConnection> connectionAccessor,
		                    ICurrentContext currentContext)
			: base(connectionAccessor, currentContext)
		{
		}

		public async Task<string> GetUserNameAsync(string id)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<string>(Queries.Users.GetNameById, new { Id = id });
			}
		}

		public async Task<string> FindByUserNameAsync(string userName)
		{
			using (IDbConnection connection = OpenConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<string>(Queries.Users.GetIdByName, new { Name = userName });
			}
		}
	}
}