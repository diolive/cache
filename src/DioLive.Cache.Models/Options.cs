using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.Models
{
	public class Options
	{
		[Key]
		[Required]
		public string UserId { get; set; }

		public int PurchaseGrouping { get; set; }

		public bool ShowPlanList { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}