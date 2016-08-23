using System;

using Microsoft.AspNetCore.Http;

namespace DioLive.Cache.WebUI.Models
{
    public class ControllerHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ControllerHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? CurrentBudgetId => _httpContextAccessor.HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));
    }
}