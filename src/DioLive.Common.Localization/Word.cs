using System.Collections.Generic;
using System.Globalization;

namespace DioLive.Common.Localization
{
	public class Word
	{
		private readonly string _defaultLanguage;
		private readonly Dictionary<string, IPluralizer> _pluralizers;

		public Word(string defaultLanguage = Cultures.enUS)
		{
			_defaultLanguage = defaultLanguage;
			_pluralizers = new Dictionary<string, IPluralizer>();
		}

		public string this[int number]
		{
			get
			{
				CultureInfo culture = CultureInfo.CurrentUICulture;
				bool isInvariant = culture.Equals(CultureInfo.InvariantCulture);
				while (!isInvariant && !_pluralizers.ContainsKey(culture.Name))
				{
					culture = culture.Parent;
				}

				string cultureName = isInvariant ? _defaultLanguage : culture.Name;
				return _pluralizers[cultureName].Pluralize(number);
			}
		}

		public void AddLanguage(IPluralizer pluralizer)
		{
			_pluralizers.Add(pluralizer.Language, pluralizer);
		}
	}
}