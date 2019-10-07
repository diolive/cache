using System;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class BudgetsController : BaseController
	{
		private readonly BudgetsLogic _budgetsLogic;
		private readonly IPermissionsValidator _permissionsValidator;
		private readonly AppUserManager _userManager;

		public BudgetsController(ICurrentContext currentContext,
		                         BudgetsLogic budgetsLogic,
		                         AppUserManager userManager,
		                         IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_budgetsLogic = budgetsLogic;
			_userManager = userManager;
			_permissionsValidator = permissionsValidator;
		}

		public IActionResult Choose(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			Guid budgetId = id.Value;
			Result result = _budgetsLogic.Open(budgetId);

			if (result.IsSuccess)
			{
				CurrentContext.BudgetId = budgetId;
			}

			return ProcessResult(result, () => RedirectToAction(nameof(PurchasesController.Index), "Purchases"));
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CreateBudgetVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Result<Guid> result = _budgetsLogic.Create(model.Name);

			if (result.IsSuccess)
			{
				CurrentContext.BudgetId = result.Data;
			}

			return ProcessResult(result, () => RedirectToAction(nameof(Choose), new { Id = result.Data }));
		}

		public async Task<IActionResult> Manage(Guid? id)
		{
			if (!id.HasValue)
			{
				id = CurrentContext.BudgetId;
				if (!id.HasValue)
				{
					return RedirectToAction(nameof(HomeController.Index), "Home");
				}
			}

			Guid budgetId = id.Value;
			Result canRenameResult = await _permissionsValidator.CheckUserCanRenameBudgetAsync(budgetId, CurrentContext.UserId);

			if (!canRenameResult.IsSuccess)
			{
				return ProcessResult(canRenameResult, null);
			}

			Result<string> getNameResult = _budgetsLogic.GetName(budgetId);

			return ProcessResult(getNameResult, () => View(new ManageBudgetVM { Id = budgetId, Name = getNameResult.Data }));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Manage(ManageBudgetVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Result renameResult = _budgetsLogic.Rename(model.Id, model.Name);

			return ProcessResult(renameResult, () => RedirectToAction(nameof(HomeController.Index), "Home"));
		}

		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			Guid budgetId = id.Value;
			Result canDeleteResult = await _permissionsValidator.CheckUserCanDeleteBudgetAsync(budgetId, CurrentContext.UserId);

			if (!canDeleteResult.IsSuccess)
			{
				return ProcessResult(canDeleteResult, null);
			}

			Result<string> getNameResult = _budgetsLogic.GetName(budgetId);

			return ProcessResult(getNameResult, () => View(new ManageBudgetVM { Id = budgetId, Name = getNameResult.Data }));
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(Guid id)
		{
			Result result = _budgetsLogic.Delete(id);

			return ProcessResult(result, () => RedirectToAction(nameof(HomeController.Index), "Home"));
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

			Result result = _budgetsLogic.Share(userId, model.Access);

			return ProcessResult(result, () => RedirectToAction(nameof(Manage), new { id = model.BudgetId }));
		}
	}
}