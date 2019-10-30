using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Users;

namespace DioLive.Cache.CoreLogic
{
	public class UsersLogic : LogicBase, IUsersLogic
	{
		public UsersLogic(ICurrentContext currentContext,
		                  JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<string> GetIdByName(string userName)
		{
			var job = new FindIdJob(userName);
			return GetJobResult(job).NullMeansNotFound();
		}

		public Result<string> GetNameById(string userId)
		{
			var job = new GetNameJob(userId);
			return GetJobResult(job).NullMeansNotFound();
		}

		public Result Register(string userId, string userName)
		{
			var job = new RegisterJob(userId, userName);
			return GetJobResult(job);
		}
	}
}