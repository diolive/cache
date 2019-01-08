using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace DioLive.Cache.Storage.Legacy.Models
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
	}
}