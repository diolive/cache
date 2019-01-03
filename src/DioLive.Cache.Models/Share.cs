using System;

namespace DioLive.Cache.Models
{
	public class Share
	{
		public Guid BudgetId { get; set; }

		public string UserId { get; set; }

		public ShareAccess Access { get; set; }

		public virtual Budget Budget { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}