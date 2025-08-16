using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/budget")]
public class BudgetApiController(
    ICurrentContext currentContext,
    IBudgetsLogic budgetsLogic
) : BaseController(currentContext)
{
    [HttpPost]
    [Route("removeCurrent")]
    public IActionResult RemoveCurrent()
    {
        Result result = budgetsLogic.Delete();

        return ProcessResult(result, Ok);
    }
}