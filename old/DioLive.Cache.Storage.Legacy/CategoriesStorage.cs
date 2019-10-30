using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

using Budget = DioLive.Cache.Storage.Legacy.Models.Budget;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class CategoriesStorage : ICategoriesStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public CategoriesStorage(ApplicationDbContext db,
		                         ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<Category?> GetAsync(int id)
		{
			return await _db.Category
				.Include(c => c.Localizations)
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.SingleOrDefaultAsync(c => c.Id == id);
		}

		public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId, string? culture = null)
		{
			List<Models.Category> categories = _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.BudgetId == budgetId)
				.AsNoTracking()
				.ToList();

			if (culture != null)
			{
				foreach (Models.Category category in categories)
				{
					CategoryLocalization localization = category.Localizations.SingleOrDefault(l => l.Culture == culture);
					if (localization?.Name != null)
					{
						category.Name = localization.Name;
					}
				}
			}

			return categories
				.OrderBy(c => c.Name)
				.ToList();
		}

		public async Task<int?> GetMostPopularIdAsync(Guid budgetId)
		{
			return _db.Category
				.Include(c => c.Purchases)
				.Where(c => c.BudgetId == budgetId)
				.Select(c => new
				{
					c.Id,
					c.Purchases.Count
				})
				.OrderByDescending(c => c.Count)
				.Select(c => c.Id)
				.FirstOrDefault();
		}

		public async Task InitializeCategoriesAsync(Guid budgetId)
		{
			List<Models.Category> defaultCategories = _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToList();

			foreach (Models.Category category in defaultCategories)
			{
				category.Id = default;
				category.OwnerId = _currentContext.UserId;
				foreach (CategoryLocalization item in category.Localizations)
				{
					item.CategoryId = default;
				}

				category.BudgetId = budgetId;
				_db.Add(category);
			}

			_db.SaveChanges();
		}

		public async Task<int> AddAsync(string name, Guid budgetId)
		{
			var category = new Models.Category
			{
				Name = name,
				BudgetId = budgetId,
				OwnerId = _currentContext.UserId
			};
			_db.Add(category);
			_db.SaveChanges();

			return category.Id;
		}

		public async Task UpdateAsync(int id, int? parentId, LocalizedName[] translates, string color)
		{
			Category category = await GetAsync(id);

			category.ParentId = parentId;

			if (translates?.FirstOrDefault() != null)
			{
				category.Name = translates[0].Name;
				ICollection<CategoryLocalization> localizations = ((Models.Category) category).Localizations;

				foreach (LocalizedName translate in translates)
				{
					CategoryLocalization current = localizations.SingleOrDefault(loc => loc.Culture == translate.Culture);
					if (current == null)
					{
						if (!string.IsNullOrWhiteSpace(translate.Name))
						{
							localizations.Add(new CategoryLocalization { Culture = translate.Culture, Name = translate.Name });
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(translate.Name))
						{
							current.Name = translate.Name;
						}
						else
						{
							localizations.Remove(current);
						}
					}
				}
			}

			if (color != null)
			{
				category.Color = Convert.ToInt32(color, 16);
			}

			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			Models.Category category = _db.Category.Find(id);
			_db.Category.Remove(category);
			await _db.SaveChangesAsync();
		}

		public async Task<int?> GetLatestAsync(Guid budgetId, string purchase)
		{
			return _db.Purchase
				.Where(p => p.Name == purchase && p.BudgetId == budgetId)
				.OrderByDescending(p => p.Date)
				.Select(p => (int?) p.CategoryId)
				.FirstOrDefault();
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			return _db.Set<CategoryLocalization>()
				.Where(cl => cl.CategoryId == categoryId)
				.ToList();
		}

		public async Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, string uiCulture, int days = 0)
		{
			IReadOnlyCollection<Category> categories = await GetAllAsync(budgetId, uiCulture);
			IEnumerable<Category> rootCategories = categories.Where(c => !c.ParentId.HasValue);

			IQueryable<Purchase> query = _db.Purchase
				.Where(p => p.BudgetId == budgetId && p.Cost > 0);

			if (days > 0)
			{
				DateTime minDate = DateTime.Today.AddDays(-days);
				query = query.Where(p => p.Date >= minDate);
			}

			ILookup<int, int> costs = query.ToLookup(c => c.CategoryId, c => c.Cost);

			return rootCategories.Select(CalculateTotals).ToArray();

			CategoryWithTotals CalculateTotals(Category category)
			{
				string displayName = category.Name;
				string color = category.Color.ToString("X6");
				int totalCost = costs[category.Id].Sum();

				CategoryWithTotals[] children = categories
					.Where(c => c.ParentId == category.Id)
					.Select(CalculateTotals)
					.ToArray();

				return new CategoryWithTotals
				{
					DisplayName = displayName,
					Color = color,
					TotalCost = totalCost,
					Children = children
				};
			}
		}

		public async Task CloneCommonCategories(string userId, Guid budgetId)
		{
			Budget budget = _db.Budget
				.Include(b => b.Categories)
				.Include(b => b.Purchases)
				.ThenInclude(p => p.Category)
				.Single(b => b.Id == budgetId);

			List<Models.Purchase> purchases = budget.Purchases
				.Where(p => p.Category.OwnerId == null)
				.ToList();

			List<Models.Category> categories = _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToList();

			foreach (Models.Category cat in categories)
			{
				foreach (Models.Purchase pur in purchases.Where(p => p.CategoryId == cat.Id))
				{
					pur.Category = cat;
				}

				cat.Id = default;
				cat.OwnerId = budget.AuthorId;
				foreach (CategoryLocalization tran in cat.Localizations)
				{
					tran.CategoryId = default;
				}

				budget.Categories.Add(cat);
			}

			budget.Version = 2;

			_db.SaveChanges();
		}
	}
}