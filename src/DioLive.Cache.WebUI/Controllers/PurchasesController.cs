using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
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
		private const string BindCreate = nameof(CreatePurchaseVM.CategoryId) + "," + nameof(CreatePurchaseVM.Date) + "," + nameof(CreatePurchaseVM.Name) + "," + nameof(CreatePurchaseVM.Cost) + "," + nameof(CreatePurchaseVM.Shop) + "," + nameof(CreatePurchaseVM.Comments) + "," + nameof(CreatePurchaseVM.PlanId);
		private const string BindEdit = nameof(EditPurchaseVM.Id) + "," + nameof(EditPurchaseVM.CategoryId) + "," + nameof(EditPurchaseVM.Date) + "," + nameof(EditPurchaseVM.Name) + "," + nameof(EditPurchaseVM.Cost) + "," + nameof(EditPurchaseVM.Shop) + "," + nameof(EditPurchaseVM.Comments);

		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;
		private readonly IOptionsStorage _optionsStorage;
		private readonly IPlansStorage _plansStorage;
		private readonly IPurchasesStorage _purchasesStorage;
		private readonly IUsersStorage _usersStorage;

		public PurchasesController(ICurrentContext currentContext,
		                           IBudgetsStorage budgetsStorage,
		                           ICategoriesStorage categoriesStorage,
		                           IOptionsStorage optionsStorage,
		                           IPlansStorage plansStorage,
		                           IPurchasesStorage purchasesStorage,
		                           IUsersStorage usersStorage)
			: base(currentContext)
		{
			_budgetsStorage = budgetsStorage;
			_categoriesStorage = categoriesStorage;
			_optionsStorage = optionsStorage;
			_plansStorage = plansStorage;
			_purchasesStorage = purchasesStorage;
			_usersStorage = usersStorage;
		}

		// GET: Purchases
		public async Task<IActionResult> Index(string filter = null)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			(Result result, Budget budget) = await _budgetsStorage.GetAsync(budgetId.Value, ShareAccess.ReadOnly);

			IActionResult processResult = ProcessResult(result, Ok);

			if (!(processResult is OkResult))
			{
				return processResult;
			}

			ViewData["BudgetId"] = budget.Id;
			ViewData["BudgetName"] = budget.Name;
			ViewData["BudgetAuthor"] = await _usersStorage.GetUserNameAsync(budget.AuthorId);

			Options userOptions = await _optionsStorage.GetAsync();
			ViewData["PurchaseGrouping"] = userOptions.PurchaseGrouping;

			if (userOptions.ShowPlanList)
			{
				ViewData["Plans"] = (await _plansStorage.FindAllAsync(budgetId.Value))
					.OrderBy(p => p.Name)
					.Select(p => new PlanVM(p))
					.ToList()
					.AsReadOnly();
			}

			IReadOnlyCollection<Purchase> purchases = await _purchasesStorage.FindAsync(budgetId.Value, filter);
			IReadOnlyCollection<Category> categories = await _categoriesStorage.GetAllAsync(budgetId.Value, CurrentContext.UICulture);

			ReadOnlyCollection<PurchaseVM> model = purchases
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

			string authorName = await _usersStorage.GetUserNameAsync(purchase.AuthorId);
			var author = new UserVM(purchase.AuthorId, authorName);

			UserVM lastEditor;
			if (purchase.LastEditorId is null)
			{
				lastEditor = null;
			}
			else
			{
				string lastEditorName = await _usersStorage.GetUserNameAsync(purchase.LastEditorId);
				lastEditor = new UserVM(purchase.LastEditorId, lastEditorName);
			}

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

			var model = new PurchaseVM(purchase);

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

			IReadOnlyCollection<string> shops = await _purchasesStorage.GetShopsAsync(budgetId.Value);
			return Json(shops);
		}

		public async Task<IActionResult> Names(string q)
		{
			Guid? budgetId = CurrentContext.BudgetId;

			if (!budgetId.HasValue)
			{
				return Json(Array.Empty<string>());
			}

			IReadOnlyCollection<string> names = await _purchasesStorage.GetNamesAsync(budgetId.Value, q);
			return Json(names);
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
						Parent = cat.ParentId.HasValue
							? categories.Single(c => c.Id == cat.ParentId.Value).Name
							: cat.Name
					};
				})
				.ToList();

			int? selectedValue = await _categoriesStorage.GetMostPopularIdAsync(budgetId);

			ViewData["CategoryId"] = new SelectList(model, "Id", "Name", selectedValue, "Parent");
		}
	}
}