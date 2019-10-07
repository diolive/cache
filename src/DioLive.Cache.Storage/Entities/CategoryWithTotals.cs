using System.Collections.Generic;

namespace DioLive.Cache.Storage.Entities
{
	public class CategoryWithTotals
	{
		public int Id { get; set; }

		public string DisplayName { get; set; } = default!;

		public string Color { get; set; } = default!;

		public int TotalCost { get; set; }

		public IReadOnlyCollection<CategoryWithTotals> Children { get; set; } = default!;
	}
}