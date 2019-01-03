﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DioLive.Cache.Models
{
	public class Category
	{
		public Category()
		{
			Subcategories = new HashSet<Category>();
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public string OwnerId { get; set; }

		public Guid? BudgetId { get; set; }

		public int Color { get; set; }

		public int? ParentId { get; set; }

		public virtual ApplicationUser Owner { get; set; }

		public virtual Budget Budget { get; set; }

		public virtual Category Parent { get; set; }

		public virtual ICollection<Purchase> Purchases { get; set; }

		public virtual ICollection<CategoryLocalization> Localizations { get; set; }

		public virtual ICollection<Category> Subcategories { get; set; }

		public string GetLocalizedName(string currentCulture)
		{
			CategoryLocalization localization = Localizations.SingleOrDefault(loc => loc.Culture == currentCulture);
			return localization?.Name ?? Name;
		}

		public IEnumerable<Category> GetFlatTree()
		{
			yield return this;

			foreach (Category item in Subcategories.SelectMany(c => c.GetFlatTree()))
			{
				yield return item;
			}
		}

		public Category GetRoot()
		{
			Category root = this;
			while (root.Parent != null)
			{
				root = root.Parent;
			}

			return root;
		}
	}
}