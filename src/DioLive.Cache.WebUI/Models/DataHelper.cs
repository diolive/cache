using AutoMapper;

using DioLive.Cache.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class DataHelper
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DataHelper(IHttpContextAccessor httpContextAccessor,
						  UserManager<ApplicationUser> userManager,
						  IMapper mapper)
		{
			_httpContextAccessor = httpContextAccessor;

			UserManager = userManager;
			Mapper = mapper;
		}

		public string CurrentCulture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()
			.RequestCulture.UICulture.Name;

		public UserManager<ApplicationUser> UserManager { get; }

		public IMapper Mapper { get; }
	}
}