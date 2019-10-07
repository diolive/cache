using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class PurchasesController : BaseController
	{
		private readonly BudgetsLogic _budgetsLogic;
		private readonly CategoriesLogic _categoriesLogic;
		private readonly OptionsLogic _optionsLogic;
		private readonly IPermissionsValidator _permissionsValidator;
		private readonly PlansLogic _plansLogic;
		private readonly PurchasesLogic _purchasesLogic;

		private readonly AppUserManager _userManager;

		public PurchasesController(ICurrentContext currentContext,
		                           BudgetsLogic budgetsLogic,
		                           CategoriesLogic categoriesLogic,
		                           OptionsLogic optionsLogic,
		                           PlansLogic plansLogic,
		                           PurchasesLogic purchasesLogic,
		                           AppUserManager userManager,
		                           IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_budgetsLogic = budgetsLogic;
			_categoriesLogic = categoriesLogic;
			_optionsLogic = optionsLogic;
			_plansLogic = plansLogic;
			_purchasesLogic = purchasesLogic;
			_userManager = userManager;
			_permissionsValidator = permissionsValidator;
		}

		public async Task<IActionResult> Index(string? filter = null)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result<(string name, string authorId)> getBudgetResult = _budgetsLogic.GetNameAndAuthorId(budgetId.Value);
			if (!getBudgetResult.IsSuccess)
			{
				return ProcessResult(getBudgetResult, null);
			}

			ViewData["BudgetId"] = budgetId.Value;
			ViewData["BudgetName"] = getBudgetResult.Data.name;
			ViewData["BudgetAuthor"] = await _userManager.GetUserNameByIdAsync(getBudgetResult.Data.authorId);

			Result<Options> getOptionsResult = _optionsLogic.Get();
			if (!getOptionsResult.IsSuccess)
			{
				return ProcessResult(getOptionsResult, null);
			}

			ViewData["PurchaseGrouping"] = getOptionsResult.Data.PurchaseGrouping;

			if (getOptionsResult.Data.ShowPlanList)
			{
				Result<IReadOnlyCollection<Plan>> getPlansResult = _plansLogic.GetAll();
				if (!getPlansResult.IsSuccess)
				{
					return ProcessResult(getPlansResult, null);
				}

				ViewData["Plans"] = getPlansResult.Data
					.Select(p => new PlanVM(p))
					.ToList()
					.AsReadOnly();
			}

			Result<IReadOnlyCollection<(Purchase purchase, Category category)>> getPurchasesResult = _purchasesLogic.FindWithCategories(filter);

			return ProcessResult(getPurchasesResult, () => View(getPurchasesResult.Data
				.Select(p => new PurchaseVM(p.purchase, p.category))
				.ToList()
				.AsReadOnly()));
		}

		public async Task<IActionResult> Create(int? planId = null)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			ResultStatus result = await _permissionsValidator.CheckUserRightsForBudgetAsync(budgetId.Value, CurrentContext.UserId, ShareAccess.Purchases);
			if (result != ResultStatus.Success)
			{
				return ProcessResult(result, null);
			}

			var model = new CreatePurchaseVM
			{
				Date = DateTime.Today,
				PlanId = planId
			};

			if (planId.HasValue)
			{
				Result<string> getPlanNameResult = _plansLogic.GetName(planId.Value);
				if (!getPlanNameResult.IsSuccess)
				{
					return ProcessResult(getPlanNameResult, null);
				}

				model.Name = getPlanNameResult.Data;
			}

			FillCategoryList();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CreatePurchaseVM model, bool oneMore = false)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			if (!ModelState.IsValid)
			{
				FillCategoryList();
				return View(model);
			}

			Result result = _purchasesLogic.Create(model.Name, model.CategoryId, model.Date, model.Cost ?? 0, model.Shop, model.Comments, model.PlanId);
			if (!result.IsSuccess)
			{
				return ProcessResult(result, null);
			}

			if (!oneMore)
			{
				return RedirectToAction(nameof(Index));
			}

			ModelState.Clear();

			model.Comments = null;
			model.Cost = null;
			model.Name = "";
			model.PlanId = null;

			FillCategoryList();
			return View(model);
		}

		public async Task<IActionResult> Edit(Guid? id)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			if (!id.HasValue)
			{
				return NotFound();
			}

			ResultStatus result = await _permissionsValidator.CheckUserCanEditPurchaseAsync(id.Value, CurrentContext.UserId);

			if (result != ResultStatus.Success)
			{
				return ProcessResult(result, null);
			}

			Result<Purchase> getPurchaseResult = _purchasesLogic.Get(id.Value);

			if (!getPurchaseResult.IsSuccess)
			{
				return ProcessResult(getPurchaseResult, null);
			}

			Purchase purchase = getPurchaseResult.Data;

			string authorName = await _userManager.GetUserNameByIdAsync(purchase.AuthorId);
			var author = new UserVM(purchase.AuthorId, authorName);

			UserVM? lastEditor;
			if (purchase.LastEditorId is null)
			{
				lastEditor = null;
			}
			else
			{
				string lastEditorName = await _userManager.GetUserNameByIdAsync(purchase.LastEditorId);
				lastEditor = new UserVM(purchase.LastEditorId, lastEditorName);
			}

			var model = new EditPurchaseVM(purchase, author, lastEditor);

			FillCategoryList();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Guid id, EditPurchaseVM model)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			if (id != model.Id)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				FillCategoryList();
				return View(model);
			}

			Result updateResult = _purchasesLogic.Update(id, model.Name, model.CategoryId, model.Date, model.Cost, model.Shop, model.Comments);

			return ProcessResult(updateResult, () => RedirectToAction(nameof(Index)));
		}

		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			ResultStatus result = await _permissionsValidator.CheckUserCanDeletePurchaseAsync(id.Value, CurrentContext.UserId);

			if (result != ResultStatus.Success)
			{
				return ProcessResult(result, null);
			}

			Result<Purchase> getResult = _purchasesLogic.Get(id.Value);

			return ProcessResult(getResult, () => View(new DeletePurchaseVM(getResult.Data)));
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(Guid id)
		{
			Result result = _purchasesLogic.Delete(id);

			return ProcessResult(result, () => RedirectToAction(nameof(Index)));
		}

		public IActionResult Shops()
		{
			Result<IReadOnlyCollection<string>> result = _purchasesLogic.GetShops();

			return ProcessResult(result, () => Json(result.Data));
		}

		public IActionResult Names(string q)
		{
			Result<IReadOnlyCollection<string>> result = _purchasesLogic.GetNames(q);

			return ProcessResult(result, () => Json(result.Data));
		}

		[HttpPost]
		public IActionResult AddPlan(string name)
		{
			Result<Plan> result = _plansLogic.Create(name);

			return ProcessResult(result, () => Json(new PlanVM(result.Data)));
		}

		[HttpPost]
		public IActionResult RemovePlan(int id)
		{
			Result result = _plansLogic.Delete(id);

			return ProcessResult(result, Ok);
		}

		private void FillCategoryList()
		{
			Result<IReadOnlyCollection<Category>> getCategoriesResult = _categoriesLogic.GetAll();

			IReadOnlyCollection<Category> categories = getCategoriesResult.Data;

			var model = categories
				.Select(cat =>
				{
					return new
					{
						cat.Id,
						cat.Name,
						Parent = cat.ParentId.HasValue
							? categories.Single(c => c.Id == cat.ParentId.Value).Name
							: cat.Name
					};
				})
				.ToList();

			Result<int?> getMostPopularResult = _categoriesLogic.GetMostPopularId();

			ViewData["CategoryId"] = new SelectList(model, "Id", "Name", getMostPopularResult.Data, "Parent");
		}
	}
}