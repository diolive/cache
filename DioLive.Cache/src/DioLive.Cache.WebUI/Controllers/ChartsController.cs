using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ControllerHelper _helper;

        public ChartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ControllerHelper helper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _helper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PieData(int days = 0)
        {
            var budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var budget = await _context.Budget
                .Include(b => b.Shares)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Purchases)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Localizations)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            if (budget == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User);
            if (!budget.HasRights(userId, ShareAccess.Categories))
            {
                return Forbid();
            }

            return Json(GetData(budget, days));
        }

        public async Task<IActionResult> SunburstData(int days = 0)
        {
            var budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var budget = await _context.Budget
                .Include(b => b.Shares)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Purchases)
                .Include(b => b.Categories)
                    .ThenInclude(c => c.Localizations)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            if (budget == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User);
            if (!budget.HasRights(userId, ShareAccess.Categories))
            {
                return Forbid();
            }

            return Json(new { DisplayName = "Total", Children = GetData(budget, days), Color = "FFF" });
        }

        private CategoryDisplayVM[] GetData(Budget budget, int days)
        {
            Func<Purchase, bool> purchaseCondition;
            if (days > 0)
            {
                purchaseCondition = p => p.Cost > 0 && (DateTime.Today - p.Date.Date).TotalDays <= days;
            }
            else
            {
                purchaseCondition = p => p.Cost > 0;
            }

            return budget.Categories.Select(cat =>
            {
                var vm = new CategoryDisplayVM
                {
                    TotalCost = cat.Purchases.Where(purchaseCondition).Sum(p => p.Cost),
                };
                _mapper.Map<Category, CategoryVM>(cat, vm);
                var currentCulture = HttpContext.GetCurrentCulture();
                vm.DisplayName = cat.GetLocalizedName(currentCulture);
                return vm;
            }).ToArray();
        }
    }
}