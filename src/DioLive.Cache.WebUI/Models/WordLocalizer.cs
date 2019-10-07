using System.Collections.Generic;

using DioLive.Cache.Common.Localization;

namespace DioLive.Cache.WebUI.Models
{
	public class WordLocalizer
	{
		public const string Purchases = nameof(Purchases);

		public WordLocalizer()
		{
			var purchases = new Word();
			purchases.AddLanguage(new EnglishPluralizer("purchase", "purchases"));
			purchases.AddLanguage(new RussianPluralizer("покупка", "покупки", "покупок"));

			Words.Add(Purchases, purchases);
		}

		public IDictionary<string, Word> Words { get; } = new Dictionary<string, Word>();
	}
}