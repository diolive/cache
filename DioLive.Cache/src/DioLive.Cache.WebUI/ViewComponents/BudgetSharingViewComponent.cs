using System;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.ViewComponents
{
    public class BudgetSharingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private static SelectList _accessSelectList;

        public BudgetSharingViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            _accessSelectList = new SelectList(new[] {
                new { Value = ShareAccess.ReadOnly, Title = "Read only" },
                new { Value = ShareAccess.Purchases, Title = "Purchases" },
                new { Value = ShareAccess.Purchases | ShareAccess.Categories, Title = "Purchases and categories" },
                new { Value = ShareAccess.FullAccess, Title = "Unlimited access" },
            }, "Value", "Title");
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid budgetId)
        {
            Budget budget = await _context.Budget.Include(b => b.Author).Include(b => b.Shares).ThenInclude(s => s.User)
                .SingleOrDefaultAsync(b => b.Id == budgetId);
            ViewData["Access"] = _accessSelectList;
            return View("Index", budget);
        }
    }
}