using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    public class Category
    {
        public Category()
        {
            Subcategories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [Required, StringLength(300)]
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

        public static Task<Category> GetWithShares(Data.ApplicationDbContext context, int id)
        {
            return context.Category.Include(c => c.Localizations).Include(c => c.Budget).ThenInclude(b => b.Shares)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public string GetLocalizedName(string currentCulture)
        {
            var localization = Localizations.SingleOrDefault(loc => loc.Culture == currentCulture);
            return localization?.Name ?? Name;
        }

        public IEnumerable<Category> GetFlatTree()
        {
            yield return this;
            foreach (var item in this.Subcategories.SelectMany(c => c.GetFlatTree()))
            {
                yield return item;
            }
        }
    }
}