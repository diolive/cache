namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Plan : Entities.Plan
	{
		public virtual ApplicationUser Author { get; set; }

		public virtual ApplicationUser Buyer { get; set; }

		public virtual Budget Budget { get; set; }
	}
}