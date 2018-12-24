using System;
using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.WebUI.Data;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
	public class MigrationHelper
	{
		public static void MigrateBudget(Guid id, ApplicationDbContext context)
		{
			Budget budget = context.Budget
				.Include(b => b.Categories)
				.Include(b => b.Purchases)
				.ThenInclude(p => p.Category)
				.Single(b => b.Id == id);

			if (budget.Version != 1)
			{
				throw new InvalidOperationException($"Couldn't migrate budget from version {budget.Version}.");
			}

			List<Purchase> purchases = budget.Purchases.Where(p => p.Category.OwnerId == null).ToList();

			List<Category> categories = context.Category.Include(c => c.Localizations).Where(c => c.OwnerId == null).AsNoTracking().ToList();
			foreach (Category cat in categories)
			{
				foreach (Purchase pur in purchases.Where(p => p.CategoryId == cat.Id))
				{
					pur.Category = cat;
				}

				cat.Id = default(int);
				cat.OwnerId = budget.AuthorId;
				foreach (CategoryLocalization tran in cat.Localizations)
				{
					tran.CategoryId = default(int);
				}

				budget.Categories.Add(cat);
			}

			budget.Version = 2;

			context.SaveChanges();
		}
	}
}