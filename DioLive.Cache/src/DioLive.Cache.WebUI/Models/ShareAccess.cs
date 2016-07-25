namespace DioLive.Cache.WebUI.Models
{
    public enum ShareAccess : byte
    {
        ReadOnly = 0,
        Purchases = 1,
        Categories = 2,
        FullAccess = 255,
    }
}