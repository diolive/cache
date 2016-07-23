using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using Microsoft.AspNetCore.Authorization;
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
            var purchases = _context.Purchase.Include(p => p.Category)
                .Where(p => p.AuthorId == _userManager.GetUserId(User))
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.CreateDate);

            return View(await purchases.ToListAsync());
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            FillCategoryList();
            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] CreatePurchaseVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
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
                    BudgetId = _context.Budget.First(b => b.AuthorId == userId).Id,
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
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase))
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

            Purchase purchase = await _context.Purchase.SingleOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase))
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
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            if (!HasRights(purchase))
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
            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.Id == id);

            if (!HasRights(purchase))
            {
                return Forbid();
            }

            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Shops()
        {
            var shops = _context.Budget.Include(b => b.Purchases)
                .Where(b => b.AuthorId == _userManager.GetUserId(User))
                .SelectMany(b => b.Purchases)
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

        private bool HasRights(Purchase purchase)
        {
            var userId = _userManager.GetUserId(User);

            return _context.Budget
                .Where(b => b.AuthorId == userId)
                .SelectMany(b => b.Purchases)
                .Contains(purchase);
        }

        private void FillCategoryList()
        {
            var categories = _context.Category.Where(c => c.OwnerId == null || c.OwnerId == _userManager.GetUserId(User));
            ViewData["CategoryId"] = new SelectList(categories.OrderBy(c => c.Name), nameof(Category.Id), nameof(Category.Name));
        }
    }
}