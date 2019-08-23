using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Budget : Entities.Budget
	{
		public Budget()
		{
			Categories = new HashSet<Category>();
			Purchases = new HashSet<Purchase>();
			Shares = new HashSet<Share>();
			Plans = new HashSet<Plan>();
		}

		public virtual ApplicationUser Author { get; set; }

		public virtual ICollection<Category> Categories { get; set; }

		public virtual ICollection<Purchase> Purchases { get; set; }

		public virtual ICollection<Share> Shares { get; set; }

		public virtual ICollection<Plan> Plans { get; set; }

		public bool HasRights(string userId, ShareAccess requiredAccess)
		{
			return AuthorId == userId ||
				   Shares.Any(s => s.UserId == userId && s.Access.HasFlag(requiredAccess));
		}
	}
}