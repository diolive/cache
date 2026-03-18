using DioRed.Cache.Domain;
using DioRed.Cache.Core.Contracts;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.Controllers.Api;

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