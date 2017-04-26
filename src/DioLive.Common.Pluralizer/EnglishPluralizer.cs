namespace DioLive.Common.Pluralizer
{
    public class EnglishPluralizer : ILanguagePluralizer
    {
        private readonly string _singular;
        private readonly string _plural;

        public EnglishPluralizer(string singular, string plural)
        {
            _singular = singular;
            _plural = plural;
        }

        public string Language { get; } = "en-US";

        public string Pluralize(int number)
        {
            if (number == 1 || number == -1)
            {
                return $"{number} {_singular}";
            }

            return $"{number} {_plural}";
        }
    }
}
