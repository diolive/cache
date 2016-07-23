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
        public async Task<IActionResult> Create([Bind("CategoryId,Date,Name,Amount,Shop,Comments")] CreatePurchaseVM model)
        {
            if (ModelState.IsValid)
            {
                Purchase purchase = new Purchase
                {
                    CategoryId = model.CategoryId,
                    Date = model.Date,
                    Name = model.Name,
                    Amount = model.Amount,
                    Shop = model.Shop,
                    Comments = model.Comments,
                    Id = Guid.NewGuid(),
                    AuthorId = _userManager.GetUserId(User),
                    CreateDate = DateTime.UtcNow,
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

            if (purchase.AuthorId != _userManager.GetUserId(User))
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
                Amount = purchase.Amount,
                Shop = purchase.Shop,
                Comments = purchase.Comments,
            };

            return View(model);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CategoryId,Date,Name,Amount,Shop,Comments")] EditPurchaseVM model)
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

            if (purchase.AuthorId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                purchase.CategoryId = model.CategoryId;
                purchase.Date = model.Date;
                purchase.Name = model.Name;
                purchase.Amount = model.Amount;
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

            if (purchase.AuthorId != _userManager.GetUserId(User))
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

            if (purchase.AuthorId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(Guid id)
        {
            return _context.Purchase.Any(e => e.Id == id);
        }

        private void FillCategoryList()
        {
            var categories = _context.Category.Where(c => c.OwnerId == null || c.OwnerId == _userManager.GetUserId(User));
            ViewData["CategoryId"] = new SelectList(categories.OrderBy(c => c.Name), nameof(Category.Id), nameof(Category.Name));
        }
    }
}