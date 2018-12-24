using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
	public class Plan
	{
		public int Id { get; set; }

		[Required]
		[StringLength(300)]
		public string Name { get; set; }

		[Required]
		public string AuthorId { get; set; }

		public string BuyerId { get; set; }

		public string Comments { get; set; }

		public Guid BudgetId { get; set; }

		public DateTime? BuyDate { get; set; }

		public virtual ApplicationUser Author { get; set; }

		public virtual ApplicationUser Buyer { get; set; }

		public virtual Budget Budget { get; set; }
	}
}