using DioLive.Common.Localization;

namespace DioLive.Cache.WebUI.Models
{
    public class Localization
    {
        static Localization()
        {
            PurchasesPluralizer = new Pluralizer();
            PurchasesPluralizer.AddLanguage(new EnglishPluralizer("purchase", "purchases"));
            PurchasesPluralizer.AddLanguage(new RussianPluralizer("покупка", "покупки", "покупок"));
        }

        public static Pluralizer PurchasesPluralizer { get; }
    }
}