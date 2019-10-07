using System;

namespace DioLive.Cache.Storage.Entities
{
	public class Budget
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = default!;

		public string AuthorId { get; set; } = default!;

		public byte Version { get; set; }
	}
}