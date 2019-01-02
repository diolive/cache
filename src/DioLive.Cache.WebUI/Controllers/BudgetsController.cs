using System;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class BudgetsController : BaseController
	{
		private const string Bind_Create = nameof(CreateBudgetVM.Name);
		private const string Bind_Manage = nameof(ManageBudgetVM.Id) + "," + nameof(ManageBudgetVM.Name);
		private readonly IApplicationUsersStorage _applicationUsersStorage;

		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;

		public BudgetsController(DataHelper dataHelper,
								 IBudgetsStorage budgetsStorage,
								 ICategoriesStorage categoriesStorage,
								 IApplicationUsersStorage applicationUsersStorage)
			: base(dataHelper)
		{
			_budgetsStorage = budgetsStorage;
			_categoriesStorage = categoriesStorage;
			_applicationUsersStorage = applicationUsersStorage;
		}

		public async Task<IActionResult> Choose(Guid id)
		{
			Budget budget = await _budgetsStorage.GetWithSharesAsync(id);

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
				await _budgetsStorage.MigrateAsync(id);
			}

			HttpContext.Session.SetString(nameof(SessionKeys.CurrentBudget), id.ToString());
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
				string userId = UserId;
				var budget = new Budget
				{
					Name = model.Name,
					Id = Guid.NewGuid(),
					AuthorId = userId,
					Version = 2
				};

				await _budgetsStorage.AddAsync(budget);
				await _categoriesStorage.InitializeCategoriesAsync(budget.Id, userId);

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

			Budget budget = await _budgetsStorage.GetWithSharesAsync(id.Value);
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

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Result renameResult = await _budgetsStorage.RenameAsync(id, UserId, model.Name);

			return ProcessResult(renameResult, () => RedirectToAction(nameof(HomeController.Index), "Home"), "Error occured on budget rename");
		}

		// GET: Budgets/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Budget budget) = await _budgetsStorage.GetForRemoveAsync(id.Value, UserId);

			return ProcessResult(result, () => View(budget));
		}

		// POST: Budgets/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Result result = await _budgetsStorage.RemoveAsync(id, UserId);

			return ProcessResult(result, () => RedirectToAction(nameof(HomeController.Index), "Home"), "Error occured on budget remove");
		}

		[HttpPost]
		public async Task<IActionResult> Share(NewShareVM model)
		{
			ApplicationUser targetUser = await _applicationUsersStorage.GetByUserNameAsync(model.UserName);
			if (targetUser == null)
			{
				return NotFound("User not found");
			}

			Result result = await _budgetsStorage.ShareAsync(model.BudgetId, UserId, targetUser.Id, model.Access);

			return ProcessResult(result, () => RedirectToAction(nameof(Manage), new { id = model.BudgetId }));
		}

		private bool HasRights(Budget budget, ShareAccess requiredAccess)
		{
			string userId = UserId;

			return budget.HasRights(userId, requiredAccess);
		}
	}
}