using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private const string Bind_Create = nameof(Category.Name);
        private const string Bind_Edit = nameof(Category.Id) + "," + nameof(Category.Name);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var model = new UserAndGlobalCategoriesVM
            {
                GlobalCategories = await _context.Category
                    .Where(c => c.OwnerId == null)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
            };

            Guid? budgetId = CurrentBudgetId;
            if (budgetId.HasValue)
            {
                model.UserCategories = await _context.Category
                    .Where(c => c.BudgetId == budgetId.Value)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }

            return View(model);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] Category category)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                category.OwnerId = userId;
                category.BudgetId = CurrentBudgetId.Value;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.SingleOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category))
            {
                return Forbid();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Bind_Edit)] Category model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            Category category = await _context.Category.SingleOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                category.Name = model.Name;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(model.Id))
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
            return View(model);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.SingleOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category))
            {
                return Forbid();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.SingleOrDefaultAsync(m => m.Id == id);

            if (!HasRights(category))
            {
                return Forbid();
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        private bool HasRights(Category category)
        {
            var userId = _userManager.GetUserId(User);

            return _context.Budget
                .Where(b => b.AuthorId == userId)
                .SelectMany(b => b.Categories)
                .Contains(category);
        }

        private Guid? CurrentBudgetId => HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));
    }
}