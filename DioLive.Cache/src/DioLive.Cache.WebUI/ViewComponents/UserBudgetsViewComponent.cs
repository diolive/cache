using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.ViewComponents
{
    public class UserBudgetsViewComponent : ViewComponent
    {
        private readonly DataHelper _helper;

        public UserBudgetsViewComponent(DataHelper helper)
        {
            _helper = helper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _helper.UserManager.GetUserId(this.HttpContext.User);
            var budgets = _helper.Db.Budget
                .Include(b => b.Shares)
                .Where(b => b.AuthorId == userId || b.Shares.Any(s => s.UserId == userId));
            ViewBag.UserId = userId;
            return View("Index", await budgets.ToListAsync());
        }
    }
}