using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;

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
			List<Models.Category> categories = await _db.Category
				.Include(c => c.Subcategories)
				.Include(c => c.Localizations)
				.Where(c => c.BudgetId == budgetId)
				.AsNoTracking()
				.ToListAsync();

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
			Models.Category category = await _db.Category
				.Include(c => c.Localizations)
				.Include(c => c.Budget).ThenInclude(b => b.Shares)
				.SingleOrDefaultAsync(c => c.Id == id);

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

		public async Task<int> GetMostPopularIdAsync(Guid budgetId)
		{
			var categories = await _db.Category
				.Include(c => c.Purchases)
				.Where(c => c.BudgetId == budgetId)
				.Select(c => new
				{
					c.Id,
					c.Purchases.Count
				})
				.ToListAsync();

			return categories
				.OrderByDescending(c => c.Count)
				.First()
				.Id;
		}

		public async Task InitializeCategoriesAsync(Guid budgetId)
		{
			List<Models.Category> defaultCategories = await _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToListAsync();

			foreach (Models.Category category in defaultCategories)
			{
				category.Id = default;
				category.OwnerId = _currentContext.UserId;
				foreach (Models.CategoryLocalization item in category.Localizations)
				{
					item.CategoryId = default;
				}

				category.BudgetId = budgetId;
				await _db.AddAsync(category);
			}

			await _db.SaveChangesAsync();
		}

		public async Task<int> AddAsync(string name, Guid budgetId)
		{
			var category = new Models.Category
			{
				Name = name,
				BudgetId = budgetId,
				OwnerId = _currentContext.UserId
			};
			await _db.AddAsync(category);
			await _db.SaveChangesAsync();

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
			return await _db.Purchase
				.Where(p => p.Name == purchase)
				.OrderByDescending(p => p.Date)
				.Select(p => (int?)p.CategoryId)
				.FirstOrDefaultAsync();
		}

		public async Task<IReadOnlyCollection<Category>> GetChildrenAsync(int categoryId)
		{
			return await _db.Category
				.Where(c => c.ParentId == categoryId)
				.ToListAsync();
		}

		public async Task<IReadOnlyCollection<CategoryLocalization>> GetLocalizationsAsync(int categoryId)
		{
			return await _db.Set<Models.CategoryLocalization>()
				.Where(cl => cl.CategoryId == categoryId)
				.ToListAsync();
		}

		private async Task<Result> SaveChangesAsync(int id)
		{
			try
			{
				await _db.SaveChangesAsync();
				return Result.Success;
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Category.AnyAsync(c => c.Id == id)
					? Result.Error
					: Result.NotFound;
			}
		}
	}
}