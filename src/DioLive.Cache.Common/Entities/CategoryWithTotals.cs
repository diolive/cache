using System.Collections.Generic;

namespace DioLive.Cache.Common.Entities
{
	public class CategoryWithTotals
	{
		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string Color { get; set; } = default!;

		public decimal TotalCost { get; set; }

		public IReadOnlyCollection<CategoryWithTotals> Children { get; set; } = default!;
	}
}