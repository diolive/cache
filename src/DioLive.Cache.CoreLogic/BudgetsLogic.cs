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

		public Result<Guid> Create(string budgetName, string currencyId)
		{
			var job = new CreateJob(budgetName, currencyId);
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

		public Result<(string name, string authorName)> GetNameAndAuthor()
		{
			var job = new GetNameAndAuthorJob();
			return GetJobResult(job);
		}

		public Result<BudgetSlim> Open(Guid budgetId)
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

		public Result<IReadOnlyCollection<ShareItem>> GetShares()
		{
			var job = new GetSharesJob();
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<Budget>> GetAllAvailable()
		{
			var job = new GetAllAvailableJob();
			return GetJobResult(job);
		}

		public Result<string> GetCurrencySign()
		{
			var job = new GetCurrencySignJob();
			return GetJobResult(job);
		}
	}
}