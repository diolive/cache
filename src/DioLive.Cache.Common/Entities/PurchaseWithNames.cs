﻿namespace DioLive.Cache.Common.Entities
{
	public class PurchaseWithNames
	{
		public Purchase Purchase { get; set; } = default!;

		public string AuthorName { get; set; } = default!;

		public string? LastEditorName { get; set; }
	}
}