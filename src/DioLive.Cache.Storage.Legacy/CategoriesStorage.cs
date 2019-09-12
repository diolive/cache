using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

#pragma warning disable 1998

namespace DioLive.Cache.Storage.Legacy
{
	public class CategoriesStorage : ICategoriesStorage
	{
		private readonly ICurrentContext _currentContext;
		private readonly ApplicationDbContext _db;

		public CategoriesStorage(ApplicationDbContext db, ICurrentContext currentContext)
		{
			_db = db;
			_currentContext = currentContext;
		}

		public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId, string culture = null)
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

		public async Task<IReadOnlyCollection<Category>> GetRootsAsync(Guid budgetId, string culture)
		{
			List<Models.Category> categories = _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.BudgetId == budgetId && c.ParentId == null)
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

		public async Task<(Result, Category)> GetAsync(int id)
		{
			Models.Category category = _db.Category
				.Include(c => c.Localizations)
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.SingleOrDefault(c => c.Id == id);

			if (category == null)
			{
				return (Result.NotFound, default);
			}

			if (category.OwnerId == null || !category.BudgetId.HasValue || !category.Budget.HasRights(_currentContext.UserId, ShareAccess.Categories))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, category);
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
				foreach (Models.CategoryLocalization item in category.Localizations)
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

		public async Task<Result> UpdateAsync(int id, int? parentId, (string name, string culture)[] translates, string color)
		{
			(Result result, Category category) = await GetAsync(id);

			if (result != Result.Success)
			{
				return result;
			}

			category.ParentId = parentId;

			if (translates?.FirstOrDefault() != null)
			{
				category.Name = translates[0].name;
				ICollection<Models.CategoryLocalization> localizations = ((Models.Category)category).Localizations;

				foreach ((string name, string culture) translate in translates)
				{
					Models.CategoryLocalization current = localizations.SingleOrDefault(loc => loc.Culture == translate.culture);
					if (current == null)
					{
						if (!string.IsNullOrWhiteSpace(translate.name))
						{
							localizations.Add(new Models.CategoryLocalization { Culture = translate.culture, Name = translate.name });
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(translate.name))
						{
							current.Name = translate.name;
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

			return await SaveChangesAsync(id);
		}

		public async Task<Result> RemoveAsync(int id)
		{
			(Result result, Category category) = await GetAsync(id);

			if (result != Result.Success)
			{
				return result;
			}

			_db.Category.Remove((Models.Category)category);

			return await SaveChangesAsync(id);
		}

		public async Task<int?> GetLatestAsync(string purchase)
		{
			return _db.Purchase
				.Where(p => p.Name == purchase)
				.OrderByDescending(p => p.Date)
				.Select(p => (int?)p.CategoryId)
				.FirstOrDefault();
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			return _db.Set<Models.CategoryLocalization>()
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

		private async Task<Result> SaveChangesAsync(int id)
		{
			try
			{
				_db.SaveChanges();
				return Result.Success;
			}
			catch (DbUpdateConcurrencyException)
			{
				return _db.Category.Any(c => c.Id == id)
					? Result.Error
					: Result.NotFound;
			}
		}
	}
}