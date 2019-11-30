using System;

using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

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

		public string UserId => _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

		public BudgetSlim? Budget
		{
			get
			{
				Guid? id = Session.GetGuid(SessionKeys.BudgetId);
				if (id.HasValue)
				{
					var currency = Session.GetString(SessionKeys.Currency);
					return new BudgetSlim
					{
						Id = id.Value,
						Currency = currency
					};
				}
				else
				{
					return null;
				}
			}

			set
			{
				if (value is null)
				{
					Session.Remove(SessionKeys.BudgetId);
					Session.Remove(SessionKeys.Currency);
				}
				else
				{
					Session.SetGuid(SessionKeys.BudgetId, value.Id);
					Session.SetString(SessionKeys.Currency, value.Currency);
				}
			}
		}

		public Guid? BudgetId => Budget?.Id;

		private ISession Session => _httpContextAccessor.HttpContext.Session;
	}
}