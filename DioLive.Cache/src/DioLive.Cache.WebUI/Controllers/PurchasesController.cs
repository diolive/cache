using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
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
        private const string Bind_Create = nameof(CreatePurchaseVM.CategoryId) + "," + nameof(CreatePurchaseVM.Date) + "," + nameof(CreatePurchaseVM.Name) + "," + nameof(CreatePurchaseVM.Cost) + "," + nameof(CreatePurchaseVM.Shop) + "," + nameof(CreatePurchaseVM.Comments);
        private const string Bind_Edit = nameof(EditPurchaseVM.Id) + "," + nameof(EditPurchaseVM.CategoryId) + "," + nameof(EditPurchaseVM.Date) + "," + nameof(EditPurchaseVM.Name) + "," + nameof(EditPurchaseVM.Cost) + "," + nameof(EditPurchaseVM.Shop) + "," + nameof(EditPurchaseVM.Comments);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchasesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var purchases = _context.Purchase.Include(p => p.Category)
                .Where(p => p.BudgetId == budgetId.Value)
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.CreateDate);

            var budget = await _context.Budget.Include(b => b.Author).SingleOrDefaultAsync(b => b.Id == budgetId.Value);
            ViewData["BudgetId"] = budget.Id;
            ViewData["BudgetName"] = budget.Name;
            ViewData["BudgetAuthor"] = budget.Author.UserName;

            return View(await purchases.ToListAsync());
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create()
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
                Purchase purchase = new Purchase
                {
                    CategoryId = model.CategoryId,
                    Date = model.Date,
                    Name = model.Name,
                    Cost = model.Cost,
                    Shop = model.Shop,
                    Comments = model.Comments,
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    CreateDate = DateTime.UtcNow,
                    BudgetId = budgetId.Value,
                };

                _context.Add(purchase);
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

            EditPurchaseVM model = new EditPurchaseVM
            {
                Id = purchase.Id,
                CategoryId = purchase.CategoryId,
                Date = purchase.Date,
                Name = purchase.Name,
                Cost = purchase.Cost,
                Shop = purchase.Shop,
                Comments = purchase.Comments,
            };

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
            IQueryable<Category> categories = _context.Category.Include(c => c.Purchases);

            Guid? budgetId = CurrentBudgetId;
            if (budgetId.HasValue)
            {
                categories = categories.Where(c => c.Owner == null || c.BudgetId == budgetId.Value);
            }
            else
            {
                categories = categories.Where(c => c.Owner == null);
            }

            var result = categories.OrderByDescending(c => c.Purchases.Count).ThenBy(c => c.Name);
            ViewData["CategoryId"] = new SelectList(result, nameof(Category.Id), nameof(Category.Name));
        }

        private Guid? CurrentBudgetId => HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));
    }
}