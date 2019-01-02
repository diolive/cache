using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

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
		private readonly IMapper _mapper;
		private readonly IPlansStorage _plansStorage;
		private readonly IPurchasesStorage _purchasesStorage;

		public PurchasesController(CurrentContext currentContext,
								   IMapper mapper,
								   IPurchasesStorage purchasesStorage,
								   IBudgetsStorage budgetsStorage,
								   IApplicationUsersStorage applicationUsersStorage,
								   IPlansStorage plansStorage,
								   ICategoriesStorage categoriesStorage)
			: base(currentContext)
		{
			_mapper = mapper;
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
			ViewData["BudgetAuthor"] = budget.Author.UserName;

			ApplicationUser user = await _applicationUsersStorage.GetWithOptionsAsync();
			ViewData["PurchaseGrouping"] = user.Options.PurchaseGrouping;

			if (user.Options.ShowPlanList)
			{
				ViewData["Plans"] = _mapper.Map<ICollection<PlanVM>>(budget.Plans.OrderBy(p => p.Name).ToList());
			}

			List<Purchase> entities = await _purchasesStorage.FindAsync(budgetId.Value, filter);
			List<PurchaseVM> model = entities.Select(ent => _mapper.Map<Purchase, PurchaseVM>(ent, opt => opt.AfterMap((src, dest) => { dest.Category.DisplayName = src.Category.GetLocalizedName(CurrentContext.UICulture); }))
			).ToList();

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

			return ProcessResult(result, () => View(_mapper.Map<EditPurchaseVM>(purchase)));
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
			return ProcessResult(result, () => View(purchase));
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
			return Json(_mapper.Map<PlanVM>(plan));
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

			List<Category> categories = await _categoriesStorage.GetAllAsync(budgetId);

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