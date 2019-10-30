using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class UsersStorage : StorageBase, IUsersStorage
	{
		public UsersStorage(IConnectionInfo connectionInfo,
		                    ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task AddAsync(string id, string name)
		{
			await Connection.ExecuteAsync(Queries.Users.Insert, new { Id = id, Name = name });
		}

		public async Task<string?> FindIdByNameAsync(string name)
		{
			return await Connection.QuerySingleOrDefaultAsync<string>(Queries.Users.FindIdByName, new { Name = name });
		}

		public async Task<string?> GetNameByIdAsync(string id)
		{
			return await Connection.QuerySingleOrDefaultAsync<string>(Queries.Users.GetNameById, new { Id = id });
		}
	}
}