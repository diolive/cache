using System;

using DioLive.Cache.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public abstract class BaseController : Controller
	{
		protected BaseController(ICurrentContext currentContext)
		{
			CurrentContext = currentContext;
		}

		protected ICurrentContext CurrentContext { get; }

		public IActionResult ProcessResult(ResultStatus result, Func<IActionResult>? successActionResult, string? errorMessage = null)
		{
			return result switch
			{
				ResultStatus.Success => (successActionResult?.Invoke() ?? throw new ArgumentNullException(nameof(successActionResult))),
				ResultStatus.Forbidden => Forbid(),
				ResultStatus.NotFound => NotFound(),
				ResultStatus.Error => BadRequest(errorMessage ?? "Error occured on request"),
				_ => throw new ArgumentOutOfRangeException(nameof(result))
			};
		}

		public IActionResult ProcessResult(Result result, Func<IActionResult>? successActionResult)
		{
			return ProcessResult(result.Status, successActionResult, result.ErrorMessage);
		}
	}
}