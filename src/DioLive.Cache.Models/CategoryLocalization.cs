namespace DioLive.Cache.Models
{
	public class CategoryLocalization
	{
		public int CategoryId { get; set; }

		public string Culture { get; set; }

		public string Name { get; set; }

		public virtual Category Category { get; set; }
	}
}