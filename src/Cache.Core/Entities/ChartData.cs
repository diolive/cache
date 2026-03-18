namespace DioRed.Cache.Core.Entities;

public class ChartData
{
    public ChartDataColumn[] Columns { get; set; } = default!;
    public ChartDataItem[] Data { get; set; } = default!;
}