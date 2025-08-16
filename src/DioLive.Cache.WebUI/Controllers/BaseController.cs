using DioLive.Cache.Common;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers;

public abstract class BaseController(
    ICurrentContext currentContext
) : Controller
{
    protected ICurrentContext CurrentContext { get; } = currentContext;

    protected IActionResult ProcessResult(Result result)
    {
        return ProcessResult(result, _ => Ok());
    }

    protected IActionResult ProcessResult(
        Result result,
        Func<IActionResult> successActionResult
    )
    {
        return ProcessResult(result, _ => successActionResult());
    }

    protected IActionResult ProcessResult<T>(Result<T> result)
    {
        return ProcessResult(result, _ => Ok());
    }

    protected IActionResult ProcessResult<T>(
        Result<T> result,
        Func<T, IActionResult> successActionResult
    )
    {
        if (result.IsSuccess)
        {
            return successActionResult(result.Value);
        }

        return result.Errors[0].Code switch
        {
            ErrorCodes.Forbidden => Forbid(),
            ErrorCodes.NotFound => NotFound(),
            _ => BadRequest(result.ErrorMessage)
        };
    }
}