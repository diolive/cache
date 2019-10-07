namespace DioLive.Cache.CoreLogic.Entities
{
	public class ChartData
	{
		public ChartDataColumn[] Columns { get; set; } = default!;
		public ChartDataItem[] Data { get; set; } = default!;
	}
}