using DioLive.Cache.Common;
using DioLive.Cache.Common.Localization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
			return View((_currentContext.Culture, Cultures.Supported));
		}
	}
}