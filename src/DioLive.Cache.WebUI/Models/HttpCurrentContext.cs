using System;

using DioLive.Cache.Auth;
using DioLive.Cache.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class HttpCurrentContext : ICurrentContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly AppUserManager _userManager;

		public HttpCurrentContext(IHttpContextAccessor httpContextAccessor,
		                          AppUserManager userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}

		public string Culture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public Guid? BudgetId
		{
			get => _httpContextAccessor.HttpContext.Session.SafeGetGuid(SessionKeys.CurrentBudget);
			set => _httpContextAccessor.HttpContext.Session.SetOrRemoveGuid(SessionKeys.CurrentBudget, value);
		}

		public string UserId => _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
	}
}