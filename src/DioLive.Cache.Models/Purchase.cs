using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.Models
{
	public class Purchase
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public int CategoryId { get; set; }

		public DateTime Date { get; set; }

		public int Cost { get; set; }

		public string Shop { get; set; }

		public string AuthorId { get; set; }

		public string LastEditorId { get; set; }

		public string Comments { get; set; }

		public DateTime CreateDate { get; set; }

		public Guid BudgetId { get; set; }

		public virtual Category Category { get; set; }

		public virtual ApplicationUser Author { get; set; }

		public virtual ApplicationUser LastEditor { get; set; }

		public virtual Budget Budget { get; set; }
	}
}