using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class UsersStorage : IUsersStorage
	{
		private readonly Func<SqlConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		public UsersStorage(Func<SqlConnection> connectionAccessor,
		                    ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		public async Task<string> GetUserNameAsync(string id)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return await connection.QuerySingleOrDefaultAsync<string>(Queries.Users.GetNameById, new { Id = id });
			}
		}

		public async Task<string> FindByUserNameAsync(string userName)
		{
			using (SqlConnection connection = _connectionAccessor())
			{
				return await connection.QuerySingleOrDefaultAsync<string>(Queries.Users.GetIdByName, new { Name = userName });
			}
		}
	}
}