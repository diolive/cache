namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Purchase : Common.Entities.Purchase
	{
		public virtual Category Category { get; set; } = default!;

		public virtual Budget Budget { get; set; } = default!;
	}
}