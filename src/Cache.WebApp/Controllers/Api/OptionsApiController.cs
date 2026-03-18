using System.Globalization;

using DioRed.Cache.Domain;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.WebApp.Models.OptionsViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/options")]
public class OptionsApiController(
    ICurrentContext currentContext,
    IOptionsLogic optionsLogic
) : BaseController(currentContext)
{
    [HttpPost]
    [Route("update")]
    public IActionResult Update([FromBody] UpdateOptionsVM model)
    {
        Result result = optionsLogic.Update(
            model.PurchaseGrouping,
            model.ShowPlanList
        );

        return ProcessResult(
            result,
            Ok
        );
    }

    [HttpPost]
    [Route("switchLocale")]
    [AllowAnonymous]
    public IActionResult SwitchLocale([FromBody] SwitchLocaleVM model)
    {
        var cultureInfo = new CultureInfo(model.Locale)
        {
            NumberFormat = { NumberDecimalSeparator = "." }
        };

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(cultureInfo)
            ),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            }
        );

        return Ok();
    }
}