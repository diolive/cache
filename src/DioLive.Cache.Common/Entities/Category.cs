using System;

namespace DioLive.Cache.Common.Entities
{
	public class Category
	{
		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string OwnerId { get; set; } = default!;

		public Guid? BudgetId { get; set; }

		public int Color { get; set; }

		public int? ParentId { get; set; }
	}
}