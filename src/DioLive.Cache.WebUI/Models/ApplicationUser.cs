using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Budgets = new HashSet<Budget>();
            Shares = new HashSet<Share>();
            Options = new Options { PurchaseGrouping = 2, ShowPlanList = true };
        }

        public virtual Options Options { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

        public virtual ICollection<Share> Shares { get; set; }

        public static Task<ApplicationUser> GetWithOptions(Data.ApplicationDbContext db, string id)
        {
            return db.Users
                .Include(u => u.Options)
                .SingleAsync(u => u.Id == id);
        }

        public void ValidateOptions(Data.ApplicationDbContext db)
        {
            var entry = db.Entry(Options);
            if (entry.State == EntityState.Detached)
            {
                entry.Entity.UserId = Id;
                entry.State = EntityState.Added;
            }
        }
    }
}