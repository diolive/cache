using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
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
		private readonly IBudgetsLogic _budgetsLogic;
		private readonly ICategoriesLogic _categoriesLogic;
		private readonly IOptionsLogic _optionsLogic;
		private readonly IPermissionsValidator _permissionsValidator;
		private readonly IPlansLogic _plansLogic;
		private readonly IPurchasesLogic _purchasesLogic;

		public PurchasesController(ICurrentContext currentContext,
		                           IBudgetsLogic budgetsLogic,
		                           ICategoriesLogic categoriesLogic,
		                           IOptionsLogic optionsLogic,
		                           IPlansLogic plansLogic,
		                           IPurchasesLogic purchasesLogic,
		                           IPermissionsValidator permissionsValidator)
			: base(currentContext)
		{
			_budgetsLogic = budgetsLogic;
			_categoriesLogic = categoriesLogic;
			_optionsLogic = optionsLogic;
			_plansLogic = plansLogic;
			_purchasesLogic = purchasesLogic;
			_permissionsValidator = permissionsValidator;
		}

		public IActionResult Index(string? filter = null)
		{
			BudgetSlim? budget = CurrentContext.Budget;
			if (budget is null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Result<(string name, string authorName)> getBudgetResult = _budgetsLogic.GetNameAndAuthor();
			if (!getBudgetResult.IsSuccess)
			{
				return ProcessResult(getBudgetResult, null);
			}

			Result<Options> getOptionsResult = _optionsLogic.Get();
			if (!getOptionsResult.IsSuccess)
			{
				return ProcessResult(getOptionsResult, null);
			}

			Result<IReadOnlyCollection<(Purchase purchase, Category category)>> getPurchasesResult = _purchasesLogic.FindWithCategories(filter);
			if (!getPurchasesResult.IsSuccess)
			{
				return ProcessResult(getPurchasesResult, null);
			}

			var model = new PurchasesPageVM
			{
				Purchases = getPurchasesResult.Data
					.Select(p => new PurchaseVM(p.purchase, p.category, budget.Currency))
					.ToList()
					.AsReadOnly(),
				BudgetId = budget.Id,
				BudgetName = getBudgetResult.Data.name,
				BudgetAuthor = getBudgetResult.Data.authorName,
				PurchaseGrouping = getOptionsResult.Data.PurchaseGrouping
			};

			if (getOptionsResult.Data.ShowPlanList)
			{
				Result<IReadOnlyCollection<Plan>> getPlansResult = _plansLogic.GetAll();
				if (!getPlansResult.IsSuccess)
				{
					return ProcessResult(getPlansResult, null);
				}

				model.Plans = getPlansResult.Data
					.Select(p => new PlanVM(p))
					.ToList()
					.AsReadOnly();
			}

			return View(model);
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

			Result<PurchaseWithNames> getPurchaseResult = _purchasesLogic.GetWithNames(id.Value);

			return ProcessResult(getPurchaseResult, () =>
			{
				var model = new EditPurchaseVM(getPurchaseResult.Data.Purchase, getPurchaseResult.Data.AuthorName, getPurchaseResult.Data.LastEditorName);

				FillCategoryList();

				return View(model);
			});
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

			BudgetSlim? budget = CurrentContext.Budget;
			if (budget is null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			ResultStatus result = await _permissionsValidator.CheckUserCanDeletePurchaseAsync(id.Value, CurrentContext.UserId);

			if (result != ResultStatus.Success)
			{
				return ProcessResult(result, null);
			}

			Result<Purchase> getResult = _purchasesLogic.Get(id.Value);

			return ProcessResult(getResult, () => View(new DeletePurchaseVM(getResult.Data, budget.Currency)));
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