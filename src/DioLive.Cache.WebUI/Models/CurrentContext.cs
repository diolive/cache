﻿using System;
using System.Security.Claims;

using DioLive.Cache.Models;
using DioLive.Cache.Storage.Contracts;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class CurrentContext : ICurrentContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<ApplicationUser> _userManager;

		public CurrentContext(IHttpContextAccessor httpContextAccessor,
							  UserManager<ApplicationUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}

		public string UICulture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public string UserId => _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

		public Guid? BudgetId
		{
			get
			{
				string id = _httpContextAccessor.HttpContext.Session.GetString(SessionKeys.CurrentBudget);
				return id != null ? Guid.Parse(id) : default(Guid?);
			}
		}
	}
}