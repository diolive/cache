using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Plans;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic
{
	public class PlansLogic : LogicBase
	{
		public PlansLogic(ICurrentContext currentContext,
		                  JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<IReadOnlyCollection<Plan>> GetAll()
		{
			var job = new GetAllJob();
			return GetJobResult(job);
		}

		public Result<string> GetName(int planId)
		{
			var job = new GetNameJob(planId);
			return GetJobResult(job);
		}

		public Result<Plan> Create(string name)
		{
			var job = new CreateJob(name);
			return GetJobResult(job);
		}

		public Result Delete(int id)
		{
			var job = new DeleteJob(id);
			return GetJobResult(job);
		}
	}
}