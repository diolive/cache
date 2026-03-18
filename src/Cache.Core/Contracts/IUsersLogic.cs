using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IUsersLogic
{
    Result<string> GetIdByName(string userName);
    Result<string> GetNameById(string userId);
    Result Register(string userId, string userName);
}