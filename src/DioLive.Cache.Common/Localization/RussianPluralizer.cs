namespace DioLive.Cache.Common.Localization
{
	public class RussianPluralizer : IPluralizer
	{
		private readonly string _plural;
		private readonly string _several;
		private readonly string _singular;

		public RussianPluralizer(string singular, string several, string plural)
		{
			_singular = singular;
			_several = several;
			_plural = plural;
		}

		public string Language { get; } = Cultures.Russian;

		public string Pluralize(int number)
		{
			string sNumber = number.ToString().PadLeft(2, '0');

			string suffix;
			if (sNumber[^2] == '1')
			{
				suffix = _plural;
			}
			else if (sNumber[^1] == '1')
			{
				suffix = _singular;
			}
			else if (sNumber[^1] == '2' || sNumber[^1] == '3' || sNumber[^1] == '4')
			{
				suffix = _several;
			}
			else
			{
				suffix = _plural;
			}

			return $"{number:D} {suffix}";
		}
	}
}