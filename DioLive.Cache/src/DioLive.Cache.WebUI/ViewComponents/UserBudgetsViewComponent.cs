using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.ViewComponents
{
    public class UserBudgetsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserBudgetsViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(this.HttpContext.User);
            var budgets = _context.Budget.Include(b => b.Shares)
                .Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId));
            ViewBag.UserId = userId;
            return View("Index", await budgets.ToListAsync());
        }
    }
}