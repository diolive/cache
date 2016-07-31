using System.Collections.Generic;
using System.Globalization;

namespace DioLive.Common.Localization
{
    public class Pluralizer
    {
        private string defaultLanguage;
        private Dictionary<string, ILanguagePluralizer> pluralizers;

        public Pluralizer(string defaultLanguage = "en-US")
        {
            this.defaultLanguage = defaultLanguage;
            this.pluralizers = new Dictionary<string, ILanguagePluralizer>();
        }

        public void AddLanguage(ILanguagePluralizer pluralizer)
        {
            this.pluralizers.Add(pluralizer.Language, pluralizer);
        }

        public string this[int number]
        {
            get
            {
                var culture = CultureInfo.CurrentUICulture.Name;
                if (!this.pluralizers.ContainsKey(culture))
                {
                    culture = this.defaultLanguage;
                }
                return this.pluralizers[culture].Pluralize(number);
            }
        }
    }
}