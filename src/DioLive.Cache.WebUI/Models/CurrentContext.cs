using System;

using DioLive.Cache.Auth;
using DioLive.Cache.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class CurrentContext : ICurrentContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly AppUserManager _userManager;

		public CurrentContext(IHttpContextAccessor httpContextAccessor,
		                      AppUserManager userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}

		public string Culture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public Guid? BudgetId
		{
			get
			{
				string id = _httpContextAccessor.HttpContext.Session.GetString(SessionKeys.CurrentBudget);
				return id != null ? Guid.Parse(id) : default(Guid?);
			}
			set => _httpContextAccessor.HttpContext.Session.SetString(nameof(SessionKeys.CurrentBudget), value?.ToString());
		}

		public string UserId => _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
	}
}