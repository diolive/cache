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

		public string Language => "ru-RU";

		public string Pluralize(int number)
		{
			string sNumber = number.ToString().PadLeft(2, '0');

			string suffix = (sNumber[^2], sNumber[^1]) switch
			{
				('1', _) => _plural,
				(_, '1') => _singular,
				var (_, d) when d >= '2' && d <= '4' => _several,
				_ => _plural
			};

			return $"{number:D} {suffix}";
		}
	}
}