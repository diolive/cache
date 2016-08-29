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

        public async Task<IActionResult> StatData(int days, int depth, int step)
        {
            var result = await _helper.OpenCurrentBudget();

            if (result.Success)
            {
                var currentCulture = _helper.CurrentCulture;

                int daysCount = (days - 1) * step + depth;
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var minDate = tomorrow.AddDays(-daysCount);

                var purchases = result.Data.Purchases
                    .Where(p => p.Cost > 0 && p.Date >= minDate && p.Date < tomorrow)
                    .ToLookup(p => new { p.Category, p.Date });

                var categories = purchases.Select(p => p.Key.Category).Distinct().ToArray();
                var dates = Enumerable.Range(0, daysCount).Select(n => minDate.AddDays(n)).ToArray();
                var statData = new int[days][];

                for (int dy = 0; dy < statData.Length; dy++)
                {
                    statData[dy] = new int[categories.Length];
                    var dateFrom = dates[dy * step];
                    var dateTo = dateFrom.AddDays(depth);

                    for (int ct = 0; ct < categories.Length; ct++)
                    {
                        var category = categories[ct];
                        statData[dy][ct] = purchases
                            .Where(p => p.Key.Category == category && p.Key.Date >= dateFrom && p.Key.Date < dateTo)
                            .SelectMany(p => p)
                            .Sum(p => p.Cost);
                    }
                }

                return Json(new
                {
                    Columns = categories.Select(cat => new
                    {
                        Name = cat.GetLocalizedName(currentCulture),
                        Color = cat.Color.ToString("X6"),
                    }).ToArray(),
                    Data = statData.Select((stat, index) => new
                    {
                        Date = dates[index * step].ToString("yyyy-MM-dd"),
                        Values = stat,
                    }).ToArray(),
                });
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