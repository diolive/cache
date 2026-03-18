namespace DioRed.Cache.Domain;

public interface IAvatarUrlProvider
{
    string GetAvatarUrl(string email, int size);
}
