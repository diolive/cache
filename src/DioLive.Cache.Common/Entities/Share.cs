using System;

namespace DioLive.Cache.Common.Entities
{
	public class Share
	{
		public Guid BudgetId { get; set; }

		public string UserId { get; set; } = default!;

		public ShareAccess Access { get; set; }
	}
}