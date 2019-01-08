using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy;
using DioLive.Cache.Storage.Legacy.Models;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Budget = DioLive.Cache.Storage.Entities.Budget;
using Category = DioLive.Cache.Storage.Entities.Category;
using Plan = DioLive.Cache.Storage.Entities.Plan;
using Purchase = DioLive.Cache.Storage.Entities.Purchase;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class PurchasesController : BaseController
	{
		private const string BindCreate = nameof(CreatePurchaseVM.CategoryId) + "," + nameof(CreatePurchaseVM.Date) + "," + nameof(CreatePurchaseVM.Name) + "," + nameof(CreatePurchaseVM.Cost) + "," + nameof(CreatePurchaseVM.Shop) + "," + nameof(CreatePurchaseVM.Comments) + "," + nameof(CreatePurchaseVM.PlanId);
		private const string BindEdit = nameof(EditPurchaseVM.Id) + "," + nameof(EditPurchaseVM.CategoryId) + "," + nameof(EditPurchaseVM.Date) + "," + nameof(EditPurchaseVM.Name) + "," + nameof(EditPurchaseVM.Cost) + "," + nameof(EditPurchaseVM.Shop) + "," + nameof(EditPurchaseVM.Comments);

		private readonly ApplicationUsersStorage _applicationUsersStorage;
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;
		private readonly IPlansStorage _plansStorage;
		private readonly IPurchasesStorage _purchasesStorage;

		public PurchasesController(CurrentContext currentContext,
								   IPurchasesStorage purchasesStorage,
								   IBudgetsStorage budgetsStorage,
								   ApplicationUsersStorage applicationUsersStorage,
								   IPlansStorage plansStorage,
								   ICategoriesStorage categoriesStorage)
			: base(currentContext)
		{
			_purchasesStorage = purchasesStorage;
			_budgetsStorage = budgetsStorage;
			_applicationUsersStorage = applicationUsersStorage;
			_plansStorage = plansStorage;
			_categoriesStorage = categoriesStorage;
		}

		// GET: Purchases
		public async Task<IActionResult> Index(string filter = null)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Budget budget = await _budgetsStorage.GetDetailsAsync(budgetId.Value);

			ViewData["BudgetId"] = budget.Id;
			ViewData["BudgetName"] = budget.Name;
			ViewData["BudgetAuthor"] = (await _applicationUsersStorage.GetAsync(budget.AuthorId)).UserName;

			ApplicationUser user = await _applicationUsersStorage.GetCurrentAsync(true);
			ViewData["PurchaseGrouping"] = user.Options.PurchaseGrouping;

			if (user.Options.ShowPlanList)
			{
				ViewData["Plans"] = (await _plansStorage.FindAllAsync(budgetId.Value))
					.OrderBy(p => p.Name)
					.Select(p => new PlanVM(p))
					.ToList()
					.AsReadOnly();
			}

			Func<Purchase, bool> purchaseFilter = null;
			if (!string.IsNullOrEmpty(filter))
			{
				purchaseFilter = p => p.Name.Contains(filter);
			}

			IReadOnlyCollection<Category> categories = await _categoriesStorage.GetAllAsync(budgetId.Value, CurrentContext.UICulture);

			ReadOnlyCollection<PurchaseVM> model = (await _purchasesStorage.FindAsync(budgetId.Value, purchaseFilter))
				.Select(p => new PurchaseVM(p, categories.Single(c => c.Id == p.CategoryId)))
				.ToList()
				.AsReadOnly();

			return View(model);
		}

		// GET: Purchases/Create
		public async Task<IActionResult> Create(int? planId = null)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			(Result result, _) = await _budgetsStorage.GetAsync(budgetId.Value, ShareAccess.Purchases);

			IActionResult processResult = ProcessResult(result, Ok);
			if (!(processResult is OkResult))
			{
				return processResult;
			}

			var model = new CreatePurchaseVM { Date = DateTime.Today };
			if (planId.HasValue)
			{
				Plan plan = await _plansStorage.FindAsync(budgetId.Value, planId.Value);

				model.PlanId = planId;
				model.Name = plan?.Name;
			}

			await FillCategoryList(budgetId.Value);
			return View(model);
		}

		// POST: Purchases/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(BindCreate)] CreatePurchaseVM model, bool oneMore = false)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			if (!ModelState.IsValid)
			{
				await FillCategoryList(budgetId.Value);
				return View(model);
			}

			(Result result, _) = await _budgetsStorage.GetAsync(budgetId.Value, ShareAccess.Purchases);

			IActionResult processResult = ProcessResult(result, Ok);
			if (!(processResult is OkResult))
			{
				return processResult;
			}

			await _purchasesStorage.AddAsync(budgetId.Value, model.Name, model.CategoryId, model.Date, model.Cost ?? 0, model.Shop, model.Comments);

			if (model.PlanId.HasValue)
			{
				await _plansStorage.BuyAsync(budgetId.Value, model.PlanId.Value);
			}

			if (!oneMore)
			{
				return RedirectToAction(nameof(Index));
			}

			ModelState.Clear();

			model.Comments = null;
			model.Cost = null;
			model.Name = null;
			model.PlanId = null;

			await FillCategoryList(budgetId.Value);
			return View(model);
		}

		// GET: Purchases/Edit/5
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

			await FillCategoryList(budgetId.Value);

			(Result result, Purchase purchase) = await _purchasesStorage.GetAsync(id.Value);

			IActionResult processResult = ProcessResult(result, Ok);

			if (!(processResult is OkResult))
			{
				return processResult;
			}

			ApplicationUser author = await _applicationUsersStorage.GetAsync(purchase.AuthorId);
			ApplicationUser lastEditor = await _applicationUsersStorage.GetAsync(purchase.LastEditorId);

			var model = new EditPurchaseVM(purchase, author, lastEditor);

			return View(model);
		}

		// POST: Purchases/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind(BindEdit)] EditPurchaseVM model)
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
				await FillCategoryList(budgetId.Value);
				return View(model);
			}

			Result updateResult = await _purchasesStorage.UpdateAsync(id, model.CategoryId, model.Date, model.Name, model.Cost, model.Shop, model.Comments);

			return ProcessResult(updateResult, () => RedirectToAction(nameof(Index)), "Error occured on purchase update");
		}

		// GET: Purchases/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Purchase purchase) = await _purchasesStorage.GetAsync(id.Value);

			IActionResult processResult = ProcessResult(result, Ok);

			if (!(processResult is OkResult))
			{
				return processResult;
			}

			(_, Category category) = await _categoriesStorage.GetAsync(purchase.CategoryId);

			var model = new PurchaseVM(purchase, category);

			return View(model);
		}

		// POST: Purchases/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Result removeResult = await _purchasesStorage.RemoveAsync(id);
			return ProcessResult(removeResult, () => RedirectToAction(nameof(Index)), "Error occured on purchase remove");
		}

		public async Task<IActionResult> Shops()
		{
			Guid? budgetId = CurrentContext.BudgetId;

			if (!budgetId.HasValue)
			{
				return Json(Array.Empty<string>());
			}

			List<string> shops = await _purchasesStorage.GetShopsAsync(budgetId.Value);
			return Json(shops.ToArray());
		}

		public async Task<IActionResult> Names(string q)
		{
			Guid? budgetId = CurrentContext.BudgetId;

			if (!budgetId.HasValue)
			{
				return Json(Array.Empty<string>());
			}

			List<string> names = await _purchasesStorage.GetNamesAsync(budgetId.Value, q);
			return Json(names.ToArray());
		}

		[HttpPost]
		public async Task<IActionResult> AddPlan(string name)
		{
			Guid? budgetId = CurrentContext.BudgetId;

			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			Plan plan = await _plansStorage.AddAsync(budgetId.Value, name);

			var model = new PlanVM(plan);
			return Json(model);
		}

		[HttpPost]
		public async Task<IActionResult> RemovePlan(int id)
		{
			Guid? budgetId = CurrentContext.BudgetId;

			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			await _plansStorage.RemoveAsync(budgetId.Value, id);
			return Ok();
		}

		private async Task FillCategoryList(Guid budgetId)
		{
			string currentCulture = CurrentContext.UICulture;

			IReadOnlyCollection<Category> categories = await _categoriesStorage.GetAllAsync(budgetId, currentCulture);

			var model = categories
				.Select(cat =>
				{
					return new
					{
						cat.Id,
						cat.Name,
						Parent = cat.ParentId.HasValue ? categories.Single(c => c.Id == cat.ParentId.Value).Name : cat.Name
					};
				})
				.ToList();

			int selectedValue = await _categoriesStorage.GetMostPopularIdAsync(budgetId);

			ViewData["CategoryId"] = new SelectList(model, "Id", "Name", selectedValue, "Parent");
		}
	}
}