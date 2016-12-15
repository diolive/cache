namespace DioLive.Common.Pluralizer
{
    public class EnglishPluralizer : ILanguagePluralizer
    {
        private string singular;
        private string plural;

        public EnglishPluralizer(string singular, string plural)
        {
            this.singular = singular;
            this.plural = plural;
        }

        public string Language { get; } = "en-US";

        public string Pluralize(int number)
        {
            if (number == 1 || number == -1)
            {
                return $"{number} {this.singular}";
            }
            else
            {
                return $"{number} {this.plural}";
            }
        }
    }
}
