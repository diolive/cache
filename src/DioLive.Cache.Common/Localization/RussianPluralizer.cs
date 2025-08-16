namespace DioLive.Cache.Common.Localization;

public class RussianPluralizer(
    string singular,
    string several,
    string plural
) : IPluralizer
{
    public string Language => "ru-RU";

    public string Pluralize(int number)
    {
        string sNumber = number.ToString().PadLeft(2, '0');

        string suffix = sNumber[^2..] switch
        {
            [not '1', '1'] => singular,
            [not '1', '2'] or [not '1', '3'] or [not '1', '4'] => several,
            _ => plural
        };

        return $"{number:D} {suffix}";
    }
}