using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Models.Data;
using DioLive.Cache.Storage.Contracts;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage
{
	public class CategoriesStorage : ICategoriesStorage
	{
		private readonly ApplicationDbContext _db;

		public CategoriesStorage(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<List<Category>> GetAsync(Guid budgetId)
		{
			return await _db.Category
				.Include(c => c.Subcategories)
				.Include(c => c.Localizations)
				.Where(c => c.BudgetId == budgetId)
				.OrderBy(c => c.Name)
				.ToListAsync();
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

		public async Task InitializeCategoriesAsync(Guid budgetId, string userId)
		{
			List<Category> defaultCategories = await _db.Category
				.Include(c => c.Localizations)
				.Where(c => c.OwnerId == null)
				.AsNoTracking()
				.ToListAsync();

			foreach (Category category in defaultCategories)
			{
				category.Id = default;
				category.OwnerId = userId;
				foreach (CategoryLocalization item in category.Localizations)
				{
					item.CategoryId = default;
				}

				category.BudgetId = budgetId;
				await _db.AddAsync(category);
			}

			await _db.SaveChangesAsync();
		}

		public async Task AddAsync(Category category)
		{
			await _db.AddAsync(category);
			await _db.SaveChangesAsync();
		}

		public async Task<Result> UpdateAsync(int id, string userId, int? parentId, (string name, string culture)[] translates, string color)
		{
			Category category = await GetWithSharesAsync(id);

			if (category == null)
			{
				return Result.NotFound;
			}

			if (category.OwnerId == null || !category.BudgetId.HasValue || !category.Budget.HasRights(userId, ShareAccess.Categories))
			{
				return Result.Forbidden;
			}

			category.ParentId = parentId;

			if (translates?.FirstOrDefault() != null)
			{
				category.Name = translates[0].name;

				foreach ((string name, string culture) translate in translates)
				{
					CategoryLocalization current = category.Localizations.SingleOrDefault(loc => loc.Culture == translate.culture);
					if (current == null)
					{
						if (!string.IsNullOrWhiteSpace(translate.name))
						{
							category.Localizations.Add(new CategoryLocalization { Culture = translate.culture, Name = translate.name });
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
							category.Localizations.Remove(current);
						}
					}
				}
			}

			if (color != null)
			{
				category.Color = Convert.ToInt32(color, 16);
			}

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Category.AnyAsync(c => c.Id == id)
					? Result.Error
					: Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<(Result, Category)> GetForRemoveAsync(int id, string userId)
		{
			Category category = await GetWithSharesAsync(id);
			if (category == null)
			{
				return (Result.NotFound, default);
			}

			if (category.OwnerId == null || !category.BudgetId.HasValue || !category.Budget.HasRights(userId, ShareAccess.Categories))
			{
				return (Result.Forbidden, default);
			}

			return (Result.Success, category);
		}

		public async Task<Result> RemoveAsync(int id, string userId)
		{
			Category category = await GetWithSharesAsync(id);
			if (category == null)
			{
				return Result.NotFound;
			}

			if (category.OwnerId == null || !category.BudgetId.HasValue || !category.Budget.HasRights(userId, ShareAccess.Categories))
			{
				return Result.Forbidden;
			}

			_db.Category.Remove(category);

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return await _db.Category.AnyAsync(c => c.Id == id)
					? Result.Error
					: Result.NotFound;
			}

			return Result.Success;
		}

		public async Task<Category> GetWithSharesAsync(int id)
		{
			return await _db.Category
				.Include(c => c.Localizations)
				.Include(c => c.Budget)
				.ThenInclude(b => b.Shares)
				.SingleOrDefaultAsync(c => c.Id == id);
		}

		public async Task<int?> GetLatestAsync(string purchase)
		{
			return await _db.Purchase
				.Where(p => p.Name == purchase)
				.OrderByDescending(p => p.Date)
				.Select(p => (int?)p.CategoryId)
				.FirstOrDefaultAsync();
		}
	}
}