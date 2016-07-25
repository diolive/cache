using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Choose(Guid id)
        {
            var budget = await Get(id);

            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget, ShareAccess.ReadOnly))
            {
                return Forbid();
            }

            HttpContext.Session.SetGuid(nameof(SessionKeys.CurrentBudget), id);
            return RedirectToAction(nameof(PurchasesController.Index), "Purchases");
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
                return RedirectToAction(nameof(Choose), new { budget.Id });
            }

            return View(model);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Manage(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var budget = await Get(id.Value);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget, ShareAccess.Manage))
            {
                return Forbid();
            }

            ManageBudgetVM model = new ManageBudgetVM
            {
                Id = budget.Id,
                Name = budget.Name,
            };

            return View(model);
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

            Budget budget = await Get(id);

            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget, ShareAccess.Manage))
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
            if (!id.HasValue)
            {
                return NotFound();
            }

            var budget = await Get(id.Value);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget, ShareAccess.Delete))
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
            var budget = await Get(id);
            if (budget == null)
            {
                return NotFound();
            }

            if (!HasRights(budget, ShareAccess.Delete))
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

        private bool HasRights(Budget budget, ShareAccess requiredAccess)
        {
            var userId = _userManager.GetUserId(User);

            return budget.AuthorId == userId || budget.Shares.Any(s => s.UserId == userId && s.Access.HasFlag(requiredAccess));
        }

        private Task<Budget> Get(Guid id)
        {
            return _context.Budget.Include(b => b.Shares).SingleOrDefaultAsync(b => b.Id == id);
        }
    }
}