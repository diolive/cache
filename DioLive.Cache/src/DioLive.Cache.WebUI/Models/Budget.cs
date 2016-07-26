using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    public class Budget
    {
        public Budget()
        {
            Categories = new HashSet<Category>();
            Purchases = new HashSet<Purchase>();
            Shares = new HashSet<Share>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }

        public virtual ICollection<Share> Shares { get; set; }

        public static Task<Budget> GetWithShares(Data.ApplicationDbContext context, Guid id)
        {
            return context.Budget.Include(b => b.Shares)
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public bool HasRights(string userId, ShareAccess requiredAccess)
        {
            return this.AuthorId == userId ||
                this.Shares.Any(s => s.UserId == userId && s.Access.HasFlag(requiredAccess));
        }
    }
}