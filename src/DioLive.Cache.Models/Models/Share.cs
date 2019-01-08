namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Share : Entities.Share
	{
		public virtual Budget Budget { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}