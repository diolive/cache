namespace DioLive.Common.Localization
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

		public string Language { get; } = Cultures.ruRU;

		public string Pluralize(int number)
		{
			string sNumber = number.ToString();

			string suffix;
			if (sNumber.EndsWith("1") && !sNumber.EndsWith("11"))
			{
				suffix = _singular;
			}
			else if (sNumber.EndsWith("2") || sNumber.EndsWith("3") || sNumber.EndsWith("4"))
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