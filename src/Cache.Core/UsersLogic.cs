using DioRed.Cache.Domain;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Users;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class UsersLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IUsersLogic
{
    public Result<string> GetIdByName(string userName)
    {
        var job = new FindIdJob(userName);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result<string> GetNameById(string userId)
    {
        var job = new GetNameJob(userId);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result Register(string userId, string userName)
    {
        var job = new RegisterJob(userId, userName);
        return GetJobResult(job);
    }
}