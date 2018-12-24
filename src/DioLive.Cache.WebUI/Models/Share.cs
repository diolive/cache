using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
	public class Share
	{
		public Guid BudgetId { get; set; }

		[Required]
		public string UserId { get; set; }

		public ShareAccess Access { get; set; }

		public virtual Budget Budget { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}