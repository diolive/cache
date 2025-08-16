using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.WebUI.Models.PlanViewModels;

public class PlanVM
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsBought { get; set; }

    public static PlanVM Build(Plan plan)
    {
        return new PlanVM
        {
            Id = plan.Id,
            Name = plan.Name,
            IsBought = plan.BuyDate.HasValue
        };
    }
}