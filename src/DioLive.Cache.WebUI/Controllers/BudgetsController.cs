using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;
using DioLive.Cache.WebUI.Models.BudgetViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class BudgetsController : BaseController
	{
		private readonly IBudgetsLogic _budgetsLogic;
		private readonly ICurrenciesLogic _currenciesLogic;
		private readonly IPermissionsValidator _permissionsValidator;
		private readonly AppUserManager _userManager;

		public BudgetsController(ICurrentContext currentContext,
		                         IBudgetsLogic budgetsLogic,
		                         ICurrenciesLogic currenciesLogic,
		                         AppUserManager userManager,
		                         IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_budgetsLogic = budgetsLogic;
			_currenciesLogic = currenciesLogic;
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
			Result<BudgetSlim> result = _budgetsLogic.Open(budgetId);

			if (result.IsSuccess)
			{
				CurrentContext.Budget = result.Data;
			}

			return ProcessResult(result, () => RedirectToAction(nameof(PurchasesController.Index), "Purchases"));
		}

		public IActionResult Create()
		{
			FillCurrenciesList();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CreateBudgetVM model)
		{
			if (!ModelState.IsValid)
			{
				FillCurrenciesList();
				return View(model);
			}

			Result<Guid> result = _budgetsLogic.Create(model.Name, model.Currency);

			return ProcessResult(result, budgetId => RedirectToAction(nameof(Choose), new { Id = budgetId }));
		}

		public async Task<IActionResult> Manage()
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result canRenameResult = await _permissionsValidator.CheckUserCanRenameBudgetAsync(budgetId.Value, CurrentContext.UserId);

			Result<string> getNameResult = canRenameResult.Then(() => _budgetsLogic.GetName());

			return ProcessResult(getNameResult, name => View(new ManageBudgetVM { Id = budgetId.Value, Name = name }));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Manage(ManageBudgetVM model)
		{
			if (!ModelState.IsValid || model.Id != CurrentContext.BudgetId)
			{
				return View(model);
			}

			Result renameResult = _budgetsLogic.Rename(model.Name);

			return ProcessResult(renameResult, () => RedirectToAction(nameof(HomeController.Index), "Home"));
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

		private void FillCurrenciesList()
		{
			Result<IReadOnlyCollection<Currency>> result = _currenciesLogic.GetAll();

			if (result.IsSuccess)
			{
				ViewBag.Currency = new SelectList(result.Data, "Id", "Sign", "RUB");
			}
		}
	}
}