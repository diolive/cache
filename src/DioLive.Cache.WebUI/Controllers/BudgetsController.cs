using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class BudgetsController : BaseController
	{
		private const string Bind_Create = nameof(CreateBudgetVM.Name);
		private const string Bind_Manage = nameof(ManageBudgetVM.Id) + "," + nameof(ManageBudgetVM.Name);

		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;
		private readonly AppUserManager _userManager;

		public BudgetsController(ICurrentContext currentContext,
		                         IBudgetsStorage budgetsStorage,
		                         ICategoriesStorage categoriesStorage,
		                         AppUserManager userManager)
			: base(currentContext)
		{
			_budgetsStorage = budgetsStorage;
			_categoriesStorage = categoriesStorage;
			_userManager = userManager;
		}

		public async Task<IActionResult> Choose(Guid id)
		{
			(Result result, Budget budget) = await _budgetsStorage.GetAsync(id, ShareAccess.ReadOnly);

			IActionResult processResult = ProcessResult(result, Ok);

			if (!(processResult is OkResult))
			{
				return processResult;
			}

			HttpContext.Session.SetString(nameof(SessionKeys.CurrentBudget), id.ToString());

			if (budget.Version == 1)
			{
				await _budgetsStorage.MigrateAsync(id);
			}

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
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Guid budgetId = await _budgetsStorage.AddAsync(model.Name);

			HttpContext.Session.SetString(nameof(SessionKeys.CurrentBudget), budgetId.ToString());

			await _categoriesStorage.InitializeCategoriesAsync();

			return RedirectToAction(nameof(Choose), new { Id = budgetId });
		}

		// GET: Budgets/Edit/5
		public async Task<IActionResult> Manage(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Budget budget) = await _budgetsStorage.GetAsync(id.Value, ShareAccess.Manage);

			return ProcessResult(result, () => View(new ManageBudgetVM { Id = budget.Id, Name = budget.Name }));
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

			Result renameResult = await _budgetsStorage.RenameAsync(id, model.Name);

			return ProcessResult(renameResult, () => RedirectToAction(nameof(HomeController.Index), "Home"), "Error occured on budget rename");
		}

		// GET: Budgets/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Budget budget) = await _budgetsStorage.GetAsync(id.Value, ShareAccess.Delete);

			return ProcessResult(result, () => View(budget));
		}

		// POST: Budgets/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Result result = await _budgetsStorage.RemoveAsync(id);

			return ProcessResult(result, () => RedirectToAction(nameof(HomeController.Index), "Home"), "Error occured on budget remove");
		}

		[HttpPost]
		public async Task<IActionResult> Share(ShareVM model)
		{
			IdentityUser user = await _userManager.FindByNameAsync(model.UserName);
			if (user is null)
			{
				return NotFound("User not found");
			}

			string userId = await _userManager.GetUserIdAsync(user);
			Result result = await _budgetsStorage.ShareAsync(model.BudgetId, userId, model.Access);

			return ProcessResult(result, () => RedirectToAction(nameof(Manage), new { id = model.BudgetId }));
		}
	}
}