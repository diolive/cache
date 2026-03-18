namespace DioRed.Cache.Core.Entities;

public class ChartDataItem
{
    public string Date { get; set; } = default!;
    public decimal[] Values { get; set; } = default!;
}