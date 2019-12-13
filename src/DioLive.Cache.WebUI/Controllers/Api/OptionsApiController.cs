using System;
using System.Globalization;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models.OptionsViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers.Api
{
	[Authorize]
	[ApiController]
	[Route("api/options")]
	public class OptionsApiController : BaseController
	{
		private readonly IOptionsLogic _optionsLogic;

		public OptionsApiController(ICurrentContext currentContext,
		                            IOptionsLogic optionsLogic)
			: base(currentContext)
		{
			_optionsLogic = optionsLogic;
		}

		[HttpPost]
		[Route("update")]
		public IActionResult Update([FromBody] UpdateOptionsVM model)
		{
			Result result = _optionsLogic.Update(model.PurchaseGrouping, model.ShowPlanList);

			return ProcessResult(result, Ok);
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
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureInfo)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return Ok();
		}
	}
}