using System.Collections.Generic;
using System.Globalization;

namespace DioLive.Common.Pluralizer
{
    public class PluralizerCollection
    {
        private string defaultLanguage;
        private Dictionary<string, ILanguagePluralizer> pluralizers;

        public PluralizerCollection(string defaultLanguage = "en-US")
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