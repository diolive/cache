using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers.Api
{
	[ApiController]
	[Authorize]
	[Route("api/budget")]
	public class BudgetApiController : BaseController
	{
		private readonly IBudgetsLogic _budgetsLogic;

		public BudgetApiController(ICurrentContext currentContext,
		                           IBudgetsLogic budgetsLogic)
			: base(currentContext)
		{
			_budgetsLogic = budgetsLogic;
		}

		[HttpPost]
		[Route("removeCurrent")]
		public IActionResult RemoveCurrent()
		{
			Result result = _budgetsLogic.Delete();

			return ProcessResult(result, Ok);
		}
	}
}