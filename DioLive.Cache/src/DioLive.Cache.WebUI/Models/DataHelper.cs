using System;
using System.Threading.Tasks;

using AutoMapper;

using DioLive.Cache.WebUI.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
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

        public async Task<LoadResult<Budget>> OpenCurrentBudget()
        {
            Guid? budgetId = this.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return LoadResult<Budget>.Fail(c => c.BadRequest());
            }

            return await OpenBudget(budgetId.Value);
        }

        public async Task<LoadResult<Budget>> OpenBudget(Guid id)
        {
            var budget = await this.Db.Budget
                .Include(b => b.Shares)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Purchases)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Localizations)
                .SingleOrDefaultAsync(b => b.Id == id);

            if (budget == null)
            {
                return LoadResult<Budget>.Fail(c => c.NotFound());
            }

            string userId = this.UserManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (!budget.HasRights(userId, ShareAccess.Categories))
            {
                return LoadResult<Budget>.Fail(c => c.Forbid());
            }

            return LoadResult<Budget>.Complete(budget);
        }
    }
}