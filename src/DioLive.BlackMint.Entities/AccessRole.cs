namespace DioLive.BlackMint.Entities
{
    public enum AccessRole
    {
        None = 0,
        Audit = 8,
        Read = 16,
        Write = 24,
        Modify = 32,
        Unlimited = 64
    }
}