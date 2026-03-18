namespace DioRed.Cache.Domain.Entities;

public class Options
{
    public string UserId { get; set; } = default!;
    public int PurchaseGrouping { get; set; }
    public bool ShowPlanList { get; set; }
}