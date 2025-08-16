namespace DioLive.Cache.Common.Localization;

public class EnglishPluralizer(
    string singular,
    string plural
) : IPluralizer
{
    public string Language => "en-US";

    public string Pluralize(int number)
    {
        string suffix = Math.Abs(number) == 1 ? singular : plural;

        return $"{number} {suffix}";
    }
}