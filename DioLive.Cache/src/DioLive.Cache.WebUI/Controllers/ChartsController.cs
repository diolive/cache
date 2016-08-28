using System;
using System.Linq;
using System.Threading.Tasks;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
    public class ChartsController : Controller
    {
        private readonly DataHelper _helper;

        public ChartsController(DataHelper helper)
        {
            _helper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PieData(int days = 0)
        {
            var result = await _helper.OpenCurrentBudget();

            if (result.Success)
            {
                var data = GetCategoriesTotalsForLastDays(result.Data, days);
                return base.Json(data);
            }
            else
            {
                return result.GetActionResult(this);
            }
        }

        public async Task<IActionResult> SunburstData(int days = 0)
        {
            var result = await _helper.OpenCurrentBudget();

            if (result.Success)
            {
                var data = GetCategoriesTotalsForLastDays(result.Data, days);
                return base.Json(new { DisplayName = "Total", Children = data, Color = "FFF" });
            }
            else
            {
                return result.GetActionResult(this);
            }
        }

        private CategoryDisplayVM[] GetCategoriesTotalsForLastDays(Budget budget, int days)
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

            var currentCulture = _helper.CurrentCulture;

            return budget.Categories.Select(cat =>
            {
                var vm = new CategoryDisplayVM
                {
                    TotalCost = cat.Purchases.Where(purchaseCondition).Sum(p => p.Cost),
                };
                _helper.Mapper.Map<Category, CategoryVM>(cat, vm);
                vm.DisplayName = cat.GetLocalizedName(currentCulture);

                return vm;
            }).ToArray();
        }
    }
}