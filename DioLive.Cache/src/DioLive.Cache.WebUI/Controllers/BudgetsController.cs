using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
    public class BudgetsController : Controller
    {
        private const string Bind_Create = nameof(CreateBudgetVM.Name);
        private const string Bind_Manage = nameof(ManageBudgetVM.Id) + "," + nameof(ManageBudgetVM.Name);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BudgetsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Budgets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Bind_Create)] CreateBudgetVM model)
        {
            if (ModelState.IsValid)
            {
                Budget budget = new Budget
                {
                    Name = model.Name,
                    Id = Guid.NewGuid(),
                    AuthorId = _userManager.GetUserId(User),
                };
                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Manage(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget.SingleOrDefaultAsync(m => m.Id == id);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget))
            {
                return Forbid();
            }

            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(Guid id, [Bind(Bind_Manage)] ManageBudgetVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            Budget budget = _context.Budget.SingleOrDefault(b => b.Id == id);

            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                budget.Name = model.Name;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(model);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget.SingleOrDefaultAsync(m => m.Id == id);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget))
            {
                return Forbid();
            }

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var budget = await _context.Budget.SingleOrDefaultAsync(m => m.Id == id);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget))
            {
                return Forbid();
            }

            _context.Budget.Remove(budget);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private bool BudgetExists(Guid id)
        {
            return _context.Budget.Any(e => e.Id == id);
        }

        private bool HasRights(Budget budget)
        {
            var userId = _userManager.GetUserId(User);

            return budget.AuthorId == userId;
        }
    }
}