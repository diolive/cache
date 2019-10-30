using System;

namespace DioLive.Cache.Common.Localization
{
	public class EnglishPluralizer : IPluralizer
	{
		private readonly string _plural;
		private readonly string _singular;

		public EnglishPluralizer(string singular, string plural)
		{
			_singular = singular;
			_plural = plural;
		}

		public string Language => "en-US";

		public string Pluralize(int number)
		{
			string suffix = Math.Abs(number) == 1 ? _singular : _plural;

			return $"{number} {suffix}";
		}
	}
}