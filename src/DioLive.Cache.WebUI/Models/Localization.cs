using DioLive.Common.Pluralizer;

namespace DioLive.Cache.WebUI.Models
{
    public class Localization
    {
        static Localization()
        {
            PurchasesPluralizer = new PluralizerCollection();
            PurchasesPluralizer.AddLanguage(new EnglishPluralizer("purchase", "purchases"));
            PurchasesPluralizer.AddLanguage(new RussianPluralizer("покупка", "покупки", "покупок"));
        }

        public static PluralizerCollection PurchasesPluralizer { get; }
    }
}