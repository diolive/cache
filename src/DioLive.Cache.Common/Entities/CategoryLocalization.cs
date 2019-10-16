namespace DioLive.Cache.Common.Entities
{
	public class CategoryLocalization
	{
		public int CategoryId { get; set; }

		public string Culture { get; set; } = default!;

		public string Name { get; set; } = default!;
	}
}