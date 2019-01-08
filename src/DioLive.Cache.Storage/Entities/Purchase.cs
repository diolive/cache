using System;

namespace DioLive.Cache.Storage.Entities
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
	}
}