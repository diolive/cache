using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Budgets;

namespace DioLive.Cache.CoreLogic
{
	public class BudgetsLogic : LogicBase, IBudgetsLogic
	{
		public BudgetsLogic(ICurrentContext currentContext,
		                    JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<Guid> Create(string budgetName)
		{
			var job = new CreateJob(budgetName);
			return GetJobResult(job);
		}

		public Result Delete()
		{
			var job = new DeleteJob();
			return GetJobResult(job);
		}

		public Result<string> GetName()
		{
			var job = new GetNameJob();
			return GetJobResult(job);
		}

		public Result<(string name, string authorId)> GetNameAndAuthorId()
		{
			var job = new GetNameAndAuthorJob();
			return GetJobResult(job);
		}

		public Result Open(Guid budgetId)
		{
			var job = new OpenJob(budgetId);
			return GetJobResult(job);
		}

		public Result Rename(string newBudgetName)
		{
			var job = new RenameJob(newBudgetName);
			return GetJobResult(job);
		}

		public Result Share(string targetUserId, ShareAccess targetAccess)
		{
			var job = new ShareJob(targetUserId, targetAccess);
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<Share>> GetShares()
		{
			var job = new GetSharesJob();
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<Budget>> GetAllAvailable()
		{
			var job = new GetAllAvailableJob();
			return GetJobResult(job);
		}
	}
}