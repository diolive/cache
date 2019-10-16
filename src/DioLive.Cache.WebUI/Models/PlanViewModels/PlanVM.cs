using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.WebUI.Models.PlanViewModels
{
	public class PlanVM
	{
		public PlanVM()
		{
		}

		public PlanVM(Plan plan)
		{
			Id = plan.Id;
			Name = plan.Name;
			IsBought = plan.BuyDate.HasValue;
		}

		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public bool IsBought { get; set; }
	}
}