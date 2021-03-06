﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        public string OwnerId { get; set; }

        public Guid? BudgetId { get; set; }

        public int Color { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual Budget Budget { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }

        public virtual ICollection<CategoryLocalization> Localizations { get; set; }

        public static Task<Category> GetWithShares(Data.ApplicationDbContext context, int id)
        {
            return context.Category.Include(c => c.Localizations).Include(c => c.Budget).ThenInclude(b => b.Shares)
                .SingleOrDefaultAsync(c => c.Id == id);
        }
    }
}