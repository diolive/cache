using System;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public abstract class BaseController : Controller
	{
		protected BaseController(ICurrentContext currentContext)
		{
			CurrentContext = currentContext;
		}

		public ICurrentContext CurrentContext { get; }

		public IActionResult ProcessResult(Result result, Func<IActionResult> successActionResult, string errorMessage = null)
		{
			switch (result)
			{
				case Result.Success:
					return successActionResult();

				case Result.Forbidden:
					return Forbid();

				case Result.NotFound:
					return NotFound();

				case Result.Error:
					return BadRequest(errorMessage ?? "Error occured on request");

				default:
					throw new ArgumentOutOfRangeException(nameof(result));
			}
		}
	}
}