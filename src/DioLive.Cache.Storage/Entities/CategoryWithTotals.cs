using System.Collections.Generic;

namespace DioLive.Cache.Storage.Entities
{
	public class CategoryWithTotals
	{
		public int Id { get; set; }

		public string DisplayName { get; set; }

		public string Color { get; set; }

		public int TotalCost { get; set; }

		public IReadOnlyCollection<CategoryWithTotals> Children { get; set; }
	}
}