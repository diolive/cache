using DioRed.Cache.Domain.Localization;

namespace DioRed.Cache.WebApp.Models;

public class WordLocalizer
{
    public const string Purchases = nameof(Purchases);

    public WordLocalizer()
    {
        var purchases = new Word();
        purchases.AddLanguage(new EnglishPluralizer("purchase", "purchases"));
        purchases.AddLanguage(new RussianPluralizer("покупка", "покупки", "покупок"));

        Words.Add(Purchases, purchases);
    }

    public Dictionary<string, Word> Words { get; } = [];
}