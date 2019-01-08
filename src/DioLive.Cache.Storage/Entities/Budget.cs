using System;

namespace DioLive.Cache.Storage.Entities
{
	public class Budget
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string AuthorId { get; set; }

		public byte Version { get; set; }
	}
}