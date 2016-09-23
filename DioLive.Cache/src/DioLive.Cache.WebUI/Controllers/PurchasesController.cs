using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private const string Bind_Create = nameof(CreatePurchaseVM.CategoryId) + "," + nameof(CreatePurchaseVM.Date) + "," + nameof(CreatePurchaseVM.Name) + "," + nameof(CreatePurchaseVM.Cost) + "," + nameof(CreatePurchaseVM.Shop) + "," + nameof(CreatePurchaseVM.Comments) + "," + nameof(CreatePurchaseVM.PlanId);
        private const string Bind_Edit = nameof(EditPurchaseVM.Id) + "," + nameof(EditPurchaseVM.CategoryId) + "," + nameof(EditPurchaseVM.Date) + "," + nameof(EditPurchaseVM.Name) + "," + nameof(EditPurchaseVM.Cost) + "," + nameof(EditPurchaseVM.Shop) + "," + nameof(EditPurchaseVM.Comments);

        private readonly DataHelper _helper;

        public PurchasesController(DataHelper helper)
        {
            _helper = helper;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var purchases = _helper.Db.Purchase.Include(p => p.Category).ThenInclude(c => c.Localizations)
                .Where(p => p.BudgetId == budgetId.Value)
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.CreateDate);

            var budget = await _helper.Db.Budget
                .Include(b => b.Author)
                .Include(b => b.Plans)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            ViewData["BudgetId"] = budget.Id;
            ViewData["BudgetName"] = budget.Name;
            ViewData["BudgetAuthor"] = budget.Author.UserName;

            var userId = _helper.UserManager.GetUserId(User);
            var user = await ApplicationUser.GetWithOptions(_helper.Db, userId);
            ViewData["PurchaseGrouping"] = user.Options.PurchaseGrouping;

            if (user.Options.ShowPlanList)
            {
                ViewData["Plans"] = _helper.Mapper.Map<ICollection<PlanVM>>(budget.Plans.OrderBy(p => p.Name).ToList());
            }

            var entities = await purchases.ToListAsync();
            var model = entities.Select(ent => _helper.Mapper.Map<Purchase, PurchaseVM>(ent, opt => opt.AfterMap((src, dest) =>
                {
                    dest.Category.DisplayName = src.Category.GetLocalizedName(_helper.CurrentCulture);
                }))
                ).ToList();

            return View(model);
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create(int? planId = null)
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _helper.UserManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_helper.Db, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Purchases))
            {
                return Forbid();
            }

            var model = new CreatePurchaseVM { Date = DateTime.Today };
            if (planId.HasValue)
            {
                var plan = _helper.Db.Budget
                    .Include(b => b.Plans)
                    .Single(b => b.Id == budgetId.Value)
                    .Plans
                    .SingleOrDefault(p => p.Id == planId.Value);

                model.PlanId = planId;
                model.Name = plan.Name;
            }

            await FillCategoryList(budgetId.Value);
            return View(model);
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] CreatePurchaseVM model)
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _helper.UserManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_helper.Db, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Purchases))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                Purchase purchase = _helper.Mapper.Map<Purchase>(model);
                purchase.AuthorId = userId;
                purchase.BudgetId = budgetId.Value;

                _helper.Db.Add(purchase);

                if (model.PlanId.HasValue)
                {
                    var plan = _helper.Db.Budget
                        .Include(b => b.Plans)
                        .Single(b => b.Id == budgetId.Value)
                        .Plans
                        .SingleOrDefault(p => p.Id == model.PlanId.Value);

                    plan.BuyDate = DateTime.UtcNow;
                    plan.BuyerId = userId;
                }

                await _helper.Db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await FillCategoryList(budgetId.Value);
            return View(model);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (!id.HasValue)
            {
                return NotFound();
            }

            var purchase = await Get(id.Value);
            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase, ShareAccess.Purchases))
            {
                return Forbid();
            }

            await FillCategoryList(budgetId.Value);

            EditPurchaseVM model = _helper.Mapper.Map<EditPurchaseVM>(purchase);
            return View(model);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(Bind_Edit)] EditPurchaseVM model)
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            Purchase purchase = await Get(id);

            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase, ShareAccess.Purchases))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                purchase.CategoryId = model.CategoryId;
                purchase.Date = model.Date;
                purchase.Name = model.Name;
                purchase.Cost = model.Cost;
                purchase.Shop = model.Shop;
                purchase.Comments = model.Comments;
                purchase.LastEditorId = _helper.UserManager.GetUserId(User);

                try
                {
                    await _helper.Db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            await FillCategoryList(budgetId.Value);
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var purchase = await Get(id.Value);
            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase, ShareAccess.Purchases))
            {
                return Forbid();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var purchase = await Get(id);
            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase, ShareAccess.Purchases))
            {
                return Forbid();
            }

            _helper.Db.Purchase.Remove(purchase);
            await _helper.Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Shops()
        {
            Guid? budgetId = _helper.CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return Json(new string[0]);
            }

            var shops = _helper.Db.Purchase
                    .Where(p => p.BudgetId == budgetId.Value)
                    .Select(p => p.Shop)
                    .Distinct()
                    .Except(new string[] { null })
                    .OrderBy(s => s);

            return Json(shops.ToArray());
        }

        public IActionResult Names(string q)
        {
            Guid? budgetId = _helper.CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return Json(new string[0]);
            }

            var names = _helper.Db.Purchase
                    .Where(p => p.BudgetId == budgetId.Value && p.Name.Contains(q))
                    .Select(p => p.Name)
                    .Distinct()
                    .Except(new string[] { null })
                    .OrderBy(s => s);

            return Json(names.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> AddPlan(string name)
        {
            Guid? budgetId = _helper.CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var budget = await _helper.Db.Budget.Include(b => b.Plans)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            var plan = new Plan
            {
                Name = name,
                AuthorId = _helper.UserManager.GetUserId(User),
            };
            budget.Plans.Add(plan);
            await _helper.Db.SaveChangesAsync();
            return Json(_helper.Mapper.Map<PlanVM>(plan));
        }

        [HttpPost]
        public async Task<IActionResult> RemovePlan(int id)
        {
            Guid? budgetId = _helper.CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var plan = _helper.Db.Budget
                    .Include(b => b.Plans)
                    .Single(b => b.Id == budgetId.Value)
                    .Plans
                    .SingleOrDefault(p => p.Id == id);

            _helper.Db.Set<Plan>().Remove(plan);
            await _helper.Db.SaveChangesAsync();
            return Ok();
        }

        private bool PurchaseExists(Guid id)
        {
            return _helper.Db.Purchase.Any(e => e.Id == id);
        }

        private Task<Purchase> Get(Guid id)
        {
            return Purchase.GetWithShares(_helper.Db, id);
        }

        private bool HasRights(Purchase purchase, ShareAccess requiredAccess)
        {
            var userId = _helper.UserManager.GetUserId(User);

            return purchase.Budget.HasRights(userId, requiredAccess);
        }

        private async Task FillCategoryList(Guid budgetId)
        {
            var currentCulture = _helper.CurrentCulture;

            var categories = await _helper.Db.Category
                .Include(c => c.Subcategories)
                .Include(c => c.Localizations)
                .Where(c => c.BudgetId == budgetId /*&& !c.ParentId.HasValue*/)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var model = categories
                .Select(c => {
                    var displayName = GetCategoryDisplayName(c, currentCulture);
                    return new
                    {
                        Id = c.Id,
                        DisplayName = displayName,
                        Parent = c.Parent != null ? GetCategoryDisplayName(c.Parent, currentCulture) : displayName,
                    };
                })
                .ToList();

            var categoriesWithCost = await _helper.Db.Category
                .Include(c => c.Purchases)
                .Where(c => c.BudgetId == budgetId)
                .Select(c => new
                {
                    Id = c.Id,
                    Count = c.Purchases.Count,
                })
                .ToListAsync();

            var selectedValue = categoriesWithCost
                .OrderByDescending(c => c.Count)
                .First()
                .Id;

            ViewData["CategoryId"] = new SelectList(model, "Id", "DisplayName", selectedValue, "Parent");           
        }

        private string GetCategoryDisplayName(Category cat, string currentCulture)
        {
            return cat.Localizations
                .Where(loc => loc.Culture == currentCulture)
                .Select(loc => loc.Name)
                .DefaultIfEmpty(cat.Name)
                .First();
        }
    }
}