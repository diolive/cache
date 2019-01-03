namespace DioLive.Cache.Models
{
	public class Options
	{
		public string UserId { get; set; }

		public int PurchaseGrouping { get; set; }

		public bool ShowPlanList { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}