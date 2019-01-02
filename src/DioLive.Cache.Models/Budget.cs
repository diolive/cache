using System;
using System.Collections.Generic;
using System.Linq;

namespace DioLive.Cache.Models
{
	public class Budget
	{
		public Budget()
		{
			Categories = new HashSet<Category>();
			Purchases = new HashSet<Purchase>();
			Shares = new HashSet<Share>();
			Plans = new HashSet<Plan>();
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public string AuthorId { get; set; }

		public byte Version { get; set; }

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