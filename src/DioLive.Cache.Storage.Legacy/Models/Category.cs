using System.Collections.Generic;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Legacy.Models
{
	public class Category : Entities.Category
	{
		public virtual Budget Budget { get; set; }

		public virtual ICollection<Purchase> Purchases { get; set; }

		public virtual ICollection<CategoryLocalization> Localizations { get; set; }
	}
}