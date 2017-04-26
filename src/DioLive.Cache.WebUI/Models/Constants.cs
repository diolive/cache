namespace DioLive.Cache.WebUI.Models
{
    public static class Constants
    {
        public const string CostDisplayFormat = "{0:N0} ₽";

        public const string DateDisplayFormat = "{0:" + Binders.DateTimeModelBinder.DateFormat + "}";

        public const string DateUtcDisplayFormat = DateDisplayFormat + " UTC";
    }
}
