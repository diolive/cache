using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = _context.Purchase.Include(p => p.Category);
            return View(await purchases.ToListAsync());
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

            return View(purchase);
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
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Date,Name")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                purchase.Id = Guid.NewGuid();
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            FillCategoryList(purchase.CategoryId);
            return View(purchase);
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
            FillCategoryList(purchase.CategoryId);
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CategoryId,Date,Name")] Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
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
            FillCategoryList(purchase.CategoryId);
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

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.Id == id);
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
            ViewData["CategoryId"] = new SelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name");
        }

        private void FillCategoryList(int defaultValue)
        {
            ViewData["CategoryId"] = new SelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name", defaultValue);
        }
    }
}