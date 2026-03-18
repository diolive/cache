using DioRed.Cache.Domain;
using DioRed.Cache.Core.Exceptions;
using DioRed.Cache.Core.Jobs;

using DioRed.Common;

namespace DioRed.Cache.Core;

public abstract class LogicBase(
    ICurrentContext currentContext,
    JobSettings jobSettings
)
{
    protected ICurrentContext CurrentContext { get; } = currentContext;
    protected Guid CurrentBudget => CurrentContext.BudgetId
        ?? throw new InvalidOperationException("No opened budget");

    protected Result GetJobResult(Job job)
    {
        try
        {
            job.Settings = jobSettings;
            job.Validate(CurrentContext)();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(GetErrorCode(ex), ex.Message));
        }
    }

    protected Result<TResult> GetJobResult<TResult>(Job<TResult> job)
    {
        try
        {
            job.Settings = jobSettings;
            TResult result = job.Validate(CurrentContext)();

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Fail(GetErrorCode(ex), ex.Message);
        }
    }

    private static int GetErrorCode(Exception ex)
    {
        return ex switch
        {
            ValidationException _ => ErrorCodes.Forbidden,
            NotFoundException _ => ErrorCodes.NotFound,
            _ => ErrorCodes.UnexpectedError
        };
    }
}