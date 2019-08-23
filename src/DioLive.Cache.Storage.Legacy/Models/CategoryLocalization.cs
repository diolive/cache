namespace DioLive.Cache.Storage.Legacy.Models
{
	public class CategoryLocalization : Entities.CategoryLocalization
	{
		public virtual Category Category { get; set; }
	}
}