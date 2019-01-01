using System;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class BudgetsController : Controller
	{
		private const string Bind_Create = nameof(CreateBudgetVM.Name);
		private const string Bind_Manage = nameof(ManageBudgetVM.Id) + "," + nameof(ManageBudgetVM.Name);

		private readonly DataHelper _helper;

		public BudgetsController(DataHelper helper)
		{
			_helper = helper;
		}

		public async Task<IActionResult> Choose(Guid id)
		{
			Budget budget = await Get(id);

			if (budget == null)
			{
				return NotFound();
			}

			if (!HasRights(budget, ShareAccess.ReadOnly))
			{
				return Forbid();
			}

			if (budget.Version == 1)
			{
				MigrationHelper.MigrateBudget(id, _helper.Db);
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
				string currentUserId = _helper.UserManager.GetUserId(User);
				var budget = new Budget
				{
					Name = model.Name,
					Id = Guid.NewGuid(),
					AuthorId = currentUserId,
					Version = 2
				};

				foreach (Category c in _helper.Db.Category.Include(c => c.Localizations).Where(c => c.OwnerId == null).AsNoTracking().ToList())
				{
					c.Id = default(int);
					c.OwnerId = currentUserId;
					foreach (CategoryLocalization item in c.Localizations)
					{
						item.CategoryId = default(int);
					}

					budget.Categories.Add(c);
				}

				_helper.Db.Add(budget);
				await _helper.Db.SaveChangesAsync();
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

			Budget budget = await Get(id.Value);
			if (budget == null)
			{
				return NotFound();
			}

			if (!HasRights(budget, ShareAccess.Manage))
			{
				return Forbid();
			}

			var model = new ManageBudgetVM
			{
				Id = budget.Id,
				Name = budget.Name
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
					await _helper.Db.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BudgetExists(budget.Id))
					{
						return NotFound();
					}

					throw;
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

			Budget budget = await Get(id.Value);
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
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Budget budget = await Get(id);
			if (budget == null)
			{
				return NotFound();
			}

			if (!HasRights(budget, ShareAccess.Delete))
			{
				return Forbid();
			}

			_helper.Db.Budget.Remove(budget);
			await _helper.Db.SaveChangesAsync();
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpPost]
		public async Task<IActionResult> Share(NewShareVM model)
		{
			Budget budget = await Get(model.BudgetId);
			if (budget == null)
			{
				return NotFound("Budget not found");
			}

			if (!HasRights(budget, ShareAccess.Manage))
			{
				return Forbid();
			}

			ApplicationUser user = await _helper.Db.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == model.UserName.ToUpperInvariant());

			if (user == null)
			{
				return NotFound("User not found");
			}

			Share share = budget.Shares.SingleOrDefault(s => s.UserId == user.Id);

			if (share != null)
			{
				share.Access = model.Access;
			}
			else
			{
				budget.Shares.Add(new Share { UserId = user.Id, Access = model.Access });
			}

			await _helper.Db.SaveChangesAsync();
			return RedirectToAction(nameof(Manage), new { id = model.BudgetId });
		}

		private bool BudgetExists(Guid id)
		{
			return _helper.Db.Budget.Any(e => e.Id == id);
		}

		private Task<Budget> Get(Guid id)
		{
			return Budget.GetWithShares(_helper.Db, id);
		}

		private bool HasRights(Budget budget, ShareAccess requiredAccess)
		{
			string userId = _helper.UserManager.GetUserId(User);

			return budget.HasRights(userId, requiredAccess);
		}
	}
}