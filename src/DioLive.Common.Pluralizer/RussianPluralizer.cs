using System;

namespace DioLive.Common.Pluralizer
{
    public class RussianPluralizer : ILanguagePluralizer
    {
        private readonly string _singular;
        private readonly string _several;
        private readonly string _plural;

        public RussianPluralizer(string singular, string several, string plural)
        {
            _singular = singular;
            _several = several;
            _plural = plural;
        }

        public string Language { get; } = "ru";

        public string Pluralize(int number)
        {
            int abs = Math.Abs(number);
            int lastDigit = abs % 10;
            int beforeLastDigit = (abs % 100 - lastDigit) / 10;

            if (beforeLastDigit != 1)
            {
                if (lastDigit == 1)
                {
                    return $"{number:D} {_singular}";
                }

                if (lastDigit >= 2 && lastDigit <= 4)
                {
                    return $"{number:D} {_several}";
                }
            }

            return $"{number:D} {_plural}";
        }
    }
}