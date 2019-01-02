using System;

using AutoMapper;

using DioLive.Cache.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

namespace DioLive.Cache.WebUI.Models
{
	public class DataHelper
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DataHelper(IHttpContextAccessor httpContextAccessor,
						  UserManager<ApplicationUser> userManager,
						  IMapper mapper,
						  ILoggerFactory loggerFactory)
		{
			_httpContextAccessor = httpContextAccessor;

			UserManager = userManager;
			Mapper = mapper;
			LoggerFactory = loggerFactory;
		}

		public string CurrentCulture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public UserManager<ApplicationUser> UserManager { get; }

		public IMapper Mapper { get; }

		public ILoggerFactory LoggerFactory { get; }
	}
}