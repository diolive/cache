using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Budgets;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic
{
	public class BudgetsLogic : LogicBase
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

		public Result Delete(Guid budgetId)
		{
			var job = new DeleteJob(budgetId);
			return GetJobResult(job);
		}

		public Result<string> GetName(Guid budgetId)
		{
			var job = new GetNameJob(budgetId);
			return GetJobResult(job);
		}

		public Result<(string name, string authorId)> GetNameAndAuthorId(Guid budgetId)
		{
			var job = new GetNameAndAuthorJob(budgetId);
			return GetJobResult(job);
		}

		public Result Open(Guid budgetId)
		{
			var job = new OpenJob(budgetId);
			return GetJobResult(job);
		}

		public Result Rename(Guid budgetId, string newBudgetName)
		{
			var job = new RenameJob(budgetId, newBudgetName);
			return GetJobResult(job);
		}

		public Result Share(string targetUserId, ShareAccess targetAccess)
		{
			var job = new ShareJob(targetUserId, targetAccess);
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<Share>> GetShares(Guid budgetId)
		{
			var job = new GetSharesJob(budgetId);
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<Budget>> GetAllAvailable()
		{
			var job = new GetAllAvailableJob();
			return GetJobResult(job);
		}
	}
}