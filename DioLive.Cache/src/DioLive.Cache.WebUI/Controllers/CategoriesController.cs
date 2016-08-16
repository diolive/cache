using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private const string Bind_Create = nameof(Category.Name);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string[] _cultures;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<RequestLocalizationOptions> locOptions)
        {
            _context = context;
            _userManager = userManager;
            _cultures = locOptions.Value.SupportedUICultures.Select(culture => culture.Name).ToArray();
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var model = new UserAndGlobalCategoriesVM
            {
                GlobalCategories = await _context.Category.Include(c => c.Localizations)
                    .Where(c => c.OwnerId == null)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
            };

            Guid? budgetId = CurrentBudgetId;
            if (budgetId.HasValue)
            {
                model.UserCategories = await _context.Category.Include(c => c.Localizations)
                    .Where(c => c.BudgetId == budgetId.Value)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }

            return View(model);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _userManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_context, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Categories))
            {
                return Forbid();
            }

            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] Category category)
        {
            Guid? budgetId = CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            string userId = _userManager.GetUserId(User);
            Budget budget = await Budget.GetWithShares(_context, budgetId.Value);
            if (!budget.HasRights(userId, ShareAccess.Categories))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                category.OwnerId = userId;
                category.BudgetId = budgetId.Value;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // POST: Categories/Update
        [HttpPost]
        public async Task<IActionResult> Update(int id, string[] data)
        {
            Category category = await Get(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category, ShareAccess.Categories))
            {
                return Forbid();
            }

            if (!ModelState.IsValid || data[0] == null)
            {
                return BadRequest();
            }

            category.Name = data[0];

            for (int i = 1; i < _cultures.Length; i++)
            {
                var actualValue = category.Localizations.SingleOrDefault(loc => loc.Culture == _cultures[i]);
                if (actualValue == null)
                {
                    if (data[i] != null)
                    {
                        category.Localizations.Add(new CategoryLocalization { Culture = _cultures[i], Name = data[i] });
                    }
                }
                else
                {
                    if (data[i] != null)
                    {
                        actualValue.Name = data[i];
                    }
                    else
                    {
                        category.Localizations.Remove(actualValue);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var category = await Get(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category, ShareAccess.Categories))
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
            var category = await Get(id);
            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category, ShareAccess.Categories))
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

        private Task<Category> Get(int id)
        {
            return Category.GetWithShares(_context, id);
        }

        private bool HasRights(Category category, ShareAccess requiredAccess)
        {
            var userId = _userManager.GetUserId(User);

            return category.Budget.HasRights(userId, requiredAccess);
        }

        private Guid? CurrentBudgetId => HttpContext.Session.GetGuid(nameof(SessionKeys.CurrentBudget));
    }
}