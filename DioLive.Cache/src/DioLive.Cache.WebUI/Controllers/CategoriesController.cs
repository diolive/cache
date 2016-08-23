using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
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
        private const string Bind_Update = nameof(UpdateCategoryVM.Id) + "," + nameof(UpdateCategoryVM.Translates) + "," + nameof(UpdateCategoryVM.Color);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string[] _cultures;
        private readonly ControllerHelper _helper;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<RequestLocalizationOptions> locOptions, ControllerHelper helper)
        {
            _context = context;
            _userManager = userManager;
            _cultures = locOptions.Value.SupportedUICultures.Select(culture => culture.Name).ToArray();
            _helper = helper;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            Guid? budgetId = _helper.CurrentBudgetId;
            if (!budgetId.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var model = await _context.Category.Include(c => c.Localizations)
                .Where(c => c.BudgetId == budgetId.Value)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            Guid? budgetId = _helper.CurrentBudgetId;
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
            Guid? budgetId = _helper.CurrentBudgetId;
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
        public async Task<IActionResult> Update([Bind(Bind_Update)]UpdateCategoryVM model)
        {
            Category category = await Get(model.Id);

            if (category == null)
            {
                return NotFound();
            }

            if (!HasRights(category, ShareAccess.Categories))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (model.Translates != null && model.Translates.Length > 0 && model.Translates[0] != null)
            {
                category.Name = model.Translates[0];

                for (int i = 1; i < _cultures.Length; i++)
                {
                    var actualValue = category.Localizations.SingleOrDefault(loc => loc.Culture == _cultures[i]);
                    if (actualValue == null)
                    {
                        if (model.Translates[i] != null)
                        {
                            category.Localizations.Add(new CategoryLocalization { Culture = _cultures[i], Name = model.Translates[i] });
                        }
                    }
                    else
                    {
                        if (model.Translates[i] != null)
                        {
                            actualValue.Name = model.Translates[i];
                        }
                        else
                        {
                            category.Localizations.Remove(actualValue);
                        }
                    }
                }
            }

            if (model.Color != null)
            {
                category.Color = Convert.ToInt32(model.Color, 16);
            }

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
            if (category.OwnerId == null || category.BudgetId == null)
            {
                return false;
            }

            var userId = _userManager.GetUserId(User);

            return category.Budget.HasRights(userId, requiredAccess);
        }
    }
}