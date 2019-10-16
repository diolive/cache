using System;

namespace DioLive.Cache.Common.Entities
{
	public class Plan
	{
		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string AuthorId { get; set; } = default!;

		public string BuyerId { get; set; } = default!;

		public string Comments { get; set; } = default!;

		public Guid BudgetId { get; set; }

		public DateTime? BuyDate { get; set; }
	}
}