using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class CategoriesController : Controller
	{
		private const string Bind_Create = nameof(Category.Name);
		private const string Bind_Update = nameof(UpdateCategoryVM.Id) + "," + nameof(UpdateCategoryVM.Translates) + "," + nameof(UpdateCategoryVM.Color) + "," + nameof(UpdateCategoryVM.ParentId);
		private readonly string[] _cultures;

		private readonly DataHelper _helper;

		public CategoriesController(DataHelper helper, IOptions<RequestLocalizationOptions> locOptions)
		{
			_helper = helper;
			_cultures = locOptions.Value.SupportedUICultures.Select(culture => culture.Name).ToArray();
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			Guid? budgetId = _helper.CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			List<Category> categories = await _helper.Db.Category
				.Include(c => c.Subcategories)
				.Include(c => c.Localizations)
				.Where(c => c.BudgetId == budgetId.Value)
				.OrderBy(c => c.Name)
				.ToListAsync();

			List<CategoryWithDepthVM> model = categories
				.Where(c => !c.ParentId.HasValue)
				.SelectMany(c => c.GetFlatTree())
				.Select(c => new CategoryWithDepthVM(c))
				.ToList();

			return View(model);
		}

		// GET: Categories/Create
		public async Task<IActionResult> Create()
		{
			Guid? budgetId = _helper.CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = _helper.UserManager.GetUserId(User);
			Budget budget = await Budget.GetWithShares(_helper.Db, budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Categories))
			{
				return Forbid();
			}

			return View();
		}

		// POST: Categories/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(Bind_Create)] Category category)
		{
			Guid? budgetId = _helper.CurrentBudgetId;
			if (!budgetId.HasValue)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			string userId = _helper.UserManager.GetUserId(User);
			Budget budget = await Budget.GetWithShares(_helper.Db, budgetId.Value);
			if (!budget.HasRights(userId, ShareAccess.Categories))
			{
				return Forbid();
			}

			if (ModelState.IsValid)
			{
				category.OwnerId = userId;
				category.BudgetId = budgetId.Value;
				_helper.Db.Add(category);
				await _helper.Db.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// POST: Categories/Update
		[HttpPost]
		public async Task<IActionResult> Update([Bind(Bind_Update)] UpdateCategoryVM model)
		{
			Category category = await Get(model.Id);

			if (category == null)
			{
				return NotFound();
			}

			if (!HasRights(category, ShareAccess.Categories))
			{
				return Forbid();
			}

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			category.ParentId = model.ParentId;

			if (model.Translates != null && model.Translates.Length > 0 && model.Translates[0] != null)
			{
				category.Name = model.Translates[0];

				for (int i = 1; i < _cultures.Length; i++)
				{
					CategoryLocalization actualValue = category.Localizations.SingleOrDefault(loc => loc.Culture == _cultures[i]);
					if (actualValue == null)
					{
						if (!string.IsNullOrWhiteSpace(model.Translates[i]))
						{
							category.Localizations.Add(new CategoryLocalization { Culture = _cultures[i], Name = model.Translates[i] });
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(model.Translates[i]))
						{
							actualValue.Name = model.Translates[i];
						}
						else
						{
							category.Localizations.Remove(actualValue);
						}
					}
				}
			}

			if (model.Color != null)
			{
				category.Color = Convert.ToInt32(model.Color, 16);
			}

			try
			{
				await _helper.Db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CategoryExists(model.Id))
				{
					return NotFound();
				}

				throw;
			}

			return Ok();
		}

		// GET: Categories/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}

			Category category = await Get(id.Value);
			if (category == null)
			{
				return NotFound();
			}

			if (!HasRights(category, ShareAccess.Categories))
			{
				return Forbid();
			}

			return View(category);
		}

		// POST: Categories/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Category category = await Get(id);
			if (category == null)
			{
				return NotFound();
			}

			if (!HasRights(category, ShareAccess.Categories))
			{
				return Forbid();
			}

			_helper.Db.Category.Remove(category);
			await _helper.Db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Latest(string purchase)
		{
			List<int> categoryId = await _helper.Db.Purchase
				.Where(p => p.Name == purchase)
				.OrderByDescending(p => p.Date)
				.Select(p => p.CategoryId)
				.Take(1)
				.ToListAsync();

			if (categoryId.Count > 0)
			{
				return Ok(categoryId[0]);
			}

			return NotFound();
		}

		private bool CategoryExists(int id)
		{
			return _helper.Db.Category.Any(e => e.Id == id);
		}

		private Task<Category> Get(int id)
		{
			return Category.GetWithShares(_helper.Db, id);
		}

		private bool HasRights(Category category, ShareAccess requiredAccess)
		{
			if (category.OwnerId == null || category.BudgetId == null)
			{
				return false;
			}

			string userId = _helper.UserManager.GetUserId(User);

			return category.Budget.HasRights(userId, requiredAccess);
		}
	}
}