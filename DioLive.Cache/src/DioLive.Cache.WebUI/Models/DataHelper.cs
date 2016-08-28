using System;

using AutoMapper;

using DioLive.Cache.WebUI.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

namespace DioLive.Cache.WebUI.Models
{
    public class DataHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataHelper(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;

            Db = db;
            UserManager = userManager;
            Mapper = mapper;
            LoggerFactory = loggerFactory;
        }

        public Guid? CurrentBudgetId => _httpContextAccessor.HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));

        public string CurrentCulture => _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name;

        public ApplicationDbContext Db { get; }

        public UserManager<ApplicationUser> UserManager { get; }

        public IMapper Mapper { get; }

        public ILoggerFactory LoggerFactory { get; }
    }
}