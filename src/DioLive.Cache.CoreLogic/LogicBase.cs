using System;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.CoreLogic.Jobs;

namespace DioLive.Cache.CoreLogic
{
	public class LogicBase
	{
		private readonly JobSettings _jobSettings;

		protected LogicBase(ICurrentContext currentContext, JobSettings jobSettings)
		{
			CurrentContext = currentContext;
			_jobSettings = jobSettings;
		}

		protected ICurrentContext CurrentContext { get; }
		protected Guid CurrentBudget => CurrentContext.BudgetId ?? throw new InvalidOperationException("No opened budget");

		protected Result GetJobResult(Job job)
		{
			try
			{
				job.Settings = _jobSettings;
				job.Validate(CurrentContext)();
				return ResultStatus.Success;
			}
			catch (Exception ex)
			{
				return new Result(GetResultStatus(ex), ex.Message);
			}
		}

		protected Result<TResult> GetJobResult<TResult>(Job<TResult> job)
		{
			try
			{
				job.Settings = _jobSettings;
				TResult result = job.Validate(CurrentContext)();

				return Result.Ok(result);
			}
			catch (Exception ex)
			{
				return new Result<TResult>(GetResultStatus(ex), ex.Message);
			}
		}

		private static ResultStatus GetResultStatus(Exception ex)
		{
			return ex switch
			{
				ValidationException _ => ResultStatus.Forbidden,
				NotFoundException _ => ResultStatus.NotFound,
				_ => ResultStatus.Error
			};
		}
	}
}