using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

		public static Task<ApplicationUser> GetWithOptions(ApplicationDbContext db, string id)
		{
			return db.Users
				.Include(u => u.Options)
				.SingleAsync(u => u.Id == id);
		}

		public void ValidateOptions(ApplicationDbContext db)
		{
			EntityEntry<Options> entry = db.Entry(Options);
			if (entry.State == EntityState.Detached)
			{
				entry.Entity.UserId = Id;
				entry.State = EntityState.Added;
			}
		}
	}
}