using System;

namespace DioLive.Common.Pluralizer
{
    public class RussianPluralizer : ILanguagePluralizer
    {
        private string singular;
        private string several;
        private string plural;

        public RussianPluralizer(string singular, string several, string plural)
        {
            this.singular = singular;
            this.several = several;
            this.plural = plural;
        }

        public string Language { get; } = "ru";

        public string Pluralize(int number)
        {
            int abs = Math.Abs(number);
            int lastDigit = abs % 10;
            int beforeLastDigit = (abs % 100 - lastDigit) / 10;

            if (lastDigit == 1 && beforeLastDigit != 1)
            {
                return $"{number} {this.singular}";
            }
            else if (lastDigit >= 2 && lastDigit <= 4 && beforeLastDigit != 1)
            {
                return $"{number} {this.several}";
            }
            else
            {
                return $"{number} {this.plural}";
            }
        }
    }
}