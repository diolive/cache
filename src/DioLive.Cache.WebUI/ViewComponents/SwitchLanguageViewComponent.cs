using System.Globalization;

using DioLive.Cache.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class SwitchLanguageViewComponent : ViewComponent
	{
		private readonly ICurrentContext _currentContext;
		private readonly IOptions<RequestLocalizationOptions> _locOptions;

		public SwitchLanguageViewComponent(IOptions<RequestLocalizationOptions> locOptions,
		                                   ICurrentContext currentContext)
		{
			_locOptions = locOptions;
			_currentContext = currentContext;
		}

		public IViewComponentResult Invoke()
		{
			string currentCulture = _currentContext.Culture;
			var cultures = new SelectList(_locOptions.Value.SupportedUICultures, nameof(CultureInfo.Name), nameof(CultureInfo.DisplayName));

			return View((currentCulture, cultures));
		}
	}
}