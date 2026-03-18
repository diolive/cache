namespace DioRed.Cache.Domain.Repositories;

public interface IUsersStorage
{
    Task AddAsync(string id, string name);
    Task<string?> FindIdByNameAsync(string name);
    Task<string?> GetNameByIdAsync(string id);
}