using System.Threading.Tasks;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IUsersStorage
	{
		Task AddAsync(string id, string name);

		Task<string?> FindIdByNameAsync(string name);

		Task<string?> GetNameByIdAsync(string id);
	}
}