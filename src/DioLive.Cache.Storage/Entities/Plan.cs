using System;

namespace DioLive.Cache.Storage.Entities
{
	public class Plan
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string AuthorId { get; set; }

		public string BuyerId { get; set; }

		public string Comments { get; set; }

		public Guid BudgetId { get; set; }

		public DateTime? BuyDate { get; set; }
	}
}