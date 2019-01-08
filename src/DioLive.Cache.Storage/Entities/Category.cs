using System;

namespace DioLive.Cache.Storage.Entities
{
	public class Category
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string OwnerId { get; set; }

		public Guid? BudgetId { get; set; }

		public int Color { get; set; }

		public int? ParentId { get; set; }
	}
}