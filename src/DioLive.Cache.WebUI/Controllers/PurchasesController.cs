using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
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
		private const string BindCreate = nameof(CreatePurchaseVM.CategoryId) + "," + nameof(CreatePurchaseVM.Date) + "," + nameof(CreatePurchaseVM.Name) + "," + nameof(CreatePurchaseVM.Cost) + "," + nameof(CreatePurchaseVM.Shop) + "," + nameof(CreatePurchaseVM.Comments) + "," + nameof(CreatePurchaseVM.PlanId);
		private const string BindEdit = nameof(EditPurchaseVM.Id) + "," + nameof(EditPurchaseVM.CategoryId) + "," + nameof(EditPurchaseVM.Date) + "," + nameof(EditPurchaseVM.Name) + "," + nameof(EditPurchaseVM.Cost) + "," + nameof(EditPurchaseVM.Shop) + "," + nameof(EditPurchaseVM.Comments);
		private readonly IApplicationUsersStorage _applicationUsersStorage;
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICategoriesStorage _categoriesStorage;

		private readonly IPlansStorage _plansStorage;
		private readonly IPurchasesStorage _purchasesStorage;

		public PurchasesController(DataHelper dataHelper,
								   IPurchasesStorage purchasesStorage,
								   IBudgetsStorage budgetsStorage,
								   IApplicationUsersStorage applicationUsersStorage,
								   IPlansStorage plansStorage,
								   ICategoriesStorage categoriesStorage)
			: base(dataHelper)
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
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			Budget budget = await _budgetsStorage.GetByIdAsync(budgetId.Value);

			ViewData["BudgetId"] = budget.Id;
			ViewData["BudgetName"] = budget.Name;
			ViewData["BudgetAuthor"] = budget.Author.UserName;

			string userId = UserId;
			ApplicationUser user = await _applicationUsersStorage.GetWithOptionsAsync(userId);
			ViewData["PurchaseGrouping"] = user.Options.PurchaseGrouping;

			if (user.Options.ShowPlanList)
			{
				ViewData["Plans"] = DataHelper.Mapper.Map<ICollection<PlanVM>>(budget.Plans.OrderBy(p => p.Name).ToList());
			}

			List<Purchase> entities = await _purchasesStorage.FindAsync(budgetId.Value, filter);
			List<PurchaseVM> model = entities.Select(ent => DataHelper.Mapper.Map<Purchase, PurchaseVM>(ent, opt => opt.AfterMap((src, dest) => { dest.Category.DisplayName = src.Category.GetLocalizedName(DataHelper.CurrentCulture); }))
			).ToList();

			return View(model);
		}

		// GET: Purchases/Create
		public async Task<IActionResult> Create(int? planId = null)
		{
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = UserId;
			Budget budget = await _budgetsStorage.GetWithSharesAsync(budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Purchases))
			{
				return Forbid();
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
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = UserId;
			Budget budget = await _budgetsStorage.GetWithSharesAsync(budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Purchases))
			{
				return Forbid();
			}

			if (ModelState.IsValid)
			{
				var purchase = DataHelper.Mapper.Map<Purchase>(model);
				purchase.AuthorId = userId;
				purchase.BudgetId = budgetId.Value;

				await _purchasesStorage.AddAsync(purchase);

				if (model.PlanId.HasValue)
				{
					await _plansStorage.BuyAsync(budgetId.Value, model.PlanId.Value, userId);
				}

				if (oneMore)
				{
					ModelState.Clear();

					model.Comments = null;
					model.Cost = null;
					model.Name = null;
					model.PlanId = null;

					await FillCategoryList(budgetId.Value);
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			}

			await FillCategoryList(budgetId.Value);
			return View(model);
		}

		// GET: Purchases/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			Guid? budgetId = CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			if (!id.HasValue)
			{
				return NotFound();
			}

			await FillCategoryList(budgetId.Value);

			(Result result, Purchase purchase) = await _purchasesStorage.GetForModificationAsync(id.Value, UserId);

			return ProcessResult(result, () => View(DataHelper.Mapper.Map<EditPurchaseVM>(purchase)));
		}

		// POST: Purchases/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind(BindEdit)] EditPurchaseVM model)
		{
			Guid? budgetId = CurrentBudgetId;
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

			Result updateResult = await _purchasesStorage.UpdateAsync(id, UserId, model.CategoryId, model.Date, model.Name, model.Cost, model.Shop, model.Comments);

			return ProcessResult(updateResult, () => RedirectToAction(nameof(Index)), "Error occured on purchase update");
		}

		// GET: Purchases/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			(Result result, Purchase purchase) = await _purchasesStorage.GetForModificationAsync(id.Value, UserId);
			return ProcessResult(result, () => View(purchase));
		}

		// POST: Purchases/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Result removeResult = await _purchasesStorage.RemoveAsync(id, UserId);
			return ProcessResult(removeResult, () => RedirectToAction(nameof(Index)), "Error occured on purchase remove");
		}

		public async Task<IActionResult> Shops()
		{
			Guid? budgetId = CurrentBudgetId;

			if (!budgetId.HasValue)
			{
				return Json(Array.Empty<string>());
			}

			List<string> shops = await _purchasesStorage.GetShopsAsync(budgetId.Value);
			return Json(shops.ToArray());
		}

		public async Task<IActionResult> Names(string q)
		{
			Guid? budgetId = CurrentBudgetId;

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
			Guid? budgetId = CurrentBudgetId;

			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			Plan plan = await _plansStorage.AddAsync(budgetId.Value, name, UserId);
			return Json(DataHelper.Mapper.Map<PlanVM>(plan));
		}

		[HttpPost]
		public async Task<IActionResult> RemovePlan(int id)
		{
			Guid? budgetId = CurrentBudgetId;

			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			await _plansStorage.RemoveAsync(budgetId.Value, id);
			return Ok();
		}

		private async Task FillCategoryList(Guid budgetId)
		{
			string currentCulture = DataHelper.CurrentCulture;

			List<Category> categories = await _categoriesStorage.GetAsync(budgetId);

			var model = categories
				.Select(c =>
				{
					string displayName = GetCategoryDisplayName(c, currentCulture);
					return new
					{
						c.Id,
						DisplayName = displayName,
						Parent = c.Parent != null ? GetCategoryDisplayName(c.Parent, currentCulture) : displayName
					};
				})
				.ToList();

			int selectedValue = await _categoriesStorage.GetMostPopularIdAsync(budgetId);

			ViewData["CategoryId"] = new SelectList(model, "Id", "DisplayName", selectedValue, "Parent");
		}

		private static string GetCategoryDisplayName(Category cat, string currentCulture)
		{
			return cat.Localizations
				.Where(loc => loc.Culture == currentCulture)
				.Select(loc => loc.Name)
				.DefaultIfEmpty(cat.Name)
				.First();
		}
	}
}