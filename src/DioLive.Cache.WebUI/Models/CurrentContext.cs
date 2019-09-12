using System;

using DioLive.Cache.Storage.Contracts;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class CurrentContext : ICurrentContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public CurrentContext(IHttpContextAccessor httpContextAccessor,
							  UserManager<IdentityUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}

		public string UICulture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public Guid? BudgetId
		{
			get
			{
				string id = _httpContextAccessor.HttpContext.Session.GetString(SessionKeys.CurrentBudget);
				return id != null ? Guid.Parse(id) : default(Guid?);
			}
		}

		public string UserId => _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
	}
}