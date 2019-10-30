using DioLive.Cache.Common;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IUsersLogic
	{
		Result<string> GetIdByName(string userName);

		Result<string> GetNameById(string userId);

		Result Register(string userId, string userName);
	}
}