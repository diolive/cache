using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public PurchasesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var purchases = _context.Purchase.Include(p => p.Category).ThenInclude(c => c.Localizations)
                .Where(p => p.BudgetId == budgetId.Value)
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.CreateDate);

            var budget = await _context.Budget
                .Include(b => b.Author)
                .Include(b => b.Plans)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            ViewData["BudgetId"] = budget.Id;
            ViewData["BudgetName"] = budget.Name;
            ViewData["BudgetAuthor"] = budget.Author.UserName;

            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(u => u.Options).SingleAsync(u => u.Id == userId);
            ViewData["PurchaseGrouping"] = user.Options.PurchaseGrouping;

            if (user.Options.ShowPlanList)
            {
                ViewData["Plans"] = _mapper.Map<ICollection<PlanVM>>(budget.Plans.OrderBy(p => p.Name).ToList());
            }

            var entities = await purchases.ToListAsync();
            var model = entities.Select(ent =>
            {
                var vm = _mapper.Map<PurchaseVM>(ent);

                var localization = ent.Category.Localizations.SingleOrDefault(loc => loc.Culture == Request.HttpContext.GetCurrentCulture());
                if (localization != null)
                {
                    vm.Category.DisplayName = localization.Name;
                };

                return vm;
            }).ToList();

            return View(model);
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create(int? planId = null)
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _userManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_context, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Purchases))
            {
                return Forbid();
            }

            FillCategoryList();

            if (planId.HasValue)
            {
                var plan = _context.Budget
                    .Include(b => b.Plans)
                    .Single(b => b.Id == budgetId.Value)
                    .Plans
                    .SingleOrDefault(p => p.Id == planId.Value);

                return View(new CreatePurchaseVM { PlanId = planId, Name = plan.Name });
            }

            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] CreatePurchaseVM model)
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _userManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_context, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Purchases))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                Purchase purchase = _mapper.Map<Purchase>(model);
                purchase.AuthorId = userId;
                purchase.BudgetId = budgetId.Value;

                _context.Add(purchase);

                if (model.PlanId.HasValue)
                {
                    var plan = _context.Budget
                        .Include(b => b.Plans)
                        .Single(b => b.Id == budgetId.Value)
                        .Plans
                        .SingleOrDefault(p => p.Id == model.PlanId.Value);

                    plan.BuyDate = DateTime.UtcNow;
                    plan.BuyerId = userId;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            FillCategoryList();
            return View(model);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
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

            FillCategoryList();

            EditPurchaseVM model = _mapper.Map<EditPurchaseVM>(purchase);
            return View(model);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(Bind_Edit)] EditPurchaseVM model)
        {
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
                purchase.LastEditorId = _userManager.GetUserId(User);

                try
                {
                    await _context.SaveChangesAsync();
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

            FillCategoryList();
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

            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Shops()
        {
            Guid? budgetId = CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return Json(new string[0]);
            }

            var shops = _context.Purchase
                    .Where(p => p.BudgetId == budgetId.Value)
                    .Select(p => p.Shop)
                    .Distinct()
                    .Except(new string[] { null })
                    .OrderBy(s => s);

            return Json(shops.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> AddPlan(string name)
        {
            Guid? budgetId = CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var budget = await _context.Budget.Include(b => b.Plans)
                .SingleOrDefaultAsync(b => b.Id == budgetId.Value);

            var plan = new Plan
            {
                Name = name,
                AuthorId = _userManager.GetUserId(User),
            };
            budget.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return Json(_mapper.Map<PlanVM>(plan));
        }

        [HttpPost]
        public async Task<IActionResult> RemovePlan(int id)
        {
            Guid? budgetId = CurrentBudgetId;

            if (!budgetId.HasValue)
            {
                return BadRequest();
            }

            var plan = _context.Budget
                    .Include(b => b.Plans)
                    .Single(b => b.Id == budgetId.Value)
                    .Plans
                    .SingleOrDefault(p => p.Id == id);

            _context.Set<Plan>().Remove(plan);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool PurchaseExists(Guid id)
        {
            return _context.Purchase.Any(e => e.Id == id);
        }

        private Task<Purchase> Get(Guid id)
        {
            return Purchase.GetWithShares(_context, id);
        }

        private bool HasRights(Purchase purchase, ShareAccess requiredAccess)
        {
            var userId = _userManager.GetUserId(User);

            return purchase.Budget.HasRights(userId, requiredAccess);
        }

        private void FillCategoryList()
        {
            IQueryable<Category> categories = _context.Category.Include(c => c.Localizations).Include(c => c.Purchases);

            Guid? budgetId = CurrentBudgetId;
            if (budgetId.HasValue)
            {
                categories = categories.Where(c => c.Owner == null || c.BudgetId == budgetId.Value);
            }
            else
            {
                categories = categories.Where(c => c.Owner == null);
            }

            var currentCulture = Request.HttpContext.GetCurrentCulture();
            var allCategories = categories
                .Select(c => new
                {
                    Id = c.Id,
                    PurchasesCount = c.Purchases.Count,
                    DefaultName = c.Name,
                    Localization = c.Localizations.SingleOrDefault(loc => loc.Culture == currentCulture),
                })
                .ToList();

            var result = allCategories
                .Select(c => new
                {
                    c.Id,
                    c.PurchasesCount,
                    DisplayName = c.Localization != null ? c.Localization.Name : c.DefaultName,
                })
                .OrderByDescending(c => c.PurchasesCount)
                .ThenBy(c => c.DisplayName)
                .Select(c => new CategoryVM { Id = c.Id, DisplayName = c.DisplayName })
                .ToList();

            ViewData["CategoryId"] = new SelectList(result, nameof(CategoryVM.Id), nameof(CategoryVM.DisplayName));
        }

        private Guid? CurrentBudgetId => HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));
    }
}