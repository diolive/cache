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
                var culture = CultureInfo.CurrentUICulture;
                while (culture != CultureInfo.InvariantCulture && !this.pluralizers.ContainsKey(culture.Name))
                {
                    culture = culture.Parent;
                }

                string cultureName = (culture != CultureInfo.InvariantCulture) ? culture.Name : this.defaultLanguage;
                return this.pluralizers[cultureName].Pluralize(number);
            }
        }
    }
}