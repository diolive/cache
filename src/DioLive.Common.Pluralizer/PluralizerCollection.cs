using System.Collections.Generic;
using System.Globalization;

namespace DioLive.Common.Pluralizer
{
    public class PluralizerCollection
    {
        private readonly string _defaultLanguage;
        private readonly Dictionary<string, ILanguagePluralizer> _pluralizers;

        public PluralizerCollection(string defaultLanguage = "en-US")
        {
            _defaultLanguage = defaultLanguage;
            _pluralizers = new Dictionary<string, ILanguagePluralizer>();
        }

        public void AddLanguage(ILanguagePluralizer pluralizer)
        {
            _pluralizers.Add(pluralizer.Language, pluralizer);
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
    }
}