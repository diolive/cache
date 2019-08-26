using System.Threading.Tasks;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IUsersStorage
	{
		Task<string> GetUserNameAsync(string id);
		Task<string> FindByUserNameAsync(string userName);
	}
}