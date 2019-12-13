﻿using System;

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

		protected IActionResult ProcessResult(ResultStatus result, Func<IActionResult>? successActionResult, string? errorMessage = null)
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

		protected IActionResult ProcessResult(Result result, Func<IActionResult>? successActionResult)
		{
			return ProcessResult(result.Status, successActionResult, result.ErrorMessage);
		}

		protected IActionResult ProcessResult<T>(Result<T> result, Func<T, IActionResult>? successActionResult)
		{
			Func<IActionResult>? successAction = successActionResult is null
				? default
				: new Func<IActionResult>(() => successActionResult(result.Data));

			return ProcessResult(result.Status, successAction, result.ErrorMessage);
		}
	}
}