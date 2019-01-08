namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Purchase : Entities.Purchase
	{
		public virtual Category Category { get; set; }

		public virtual ApplicationUser Author { get; set; }

		public virtual ApplicationUser LastEditor { get; set; }

		public virtual Budget Budget { get; set; }
	}
}