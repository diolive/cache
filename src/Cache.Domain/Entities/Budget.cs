namespace DioRed.Cache.Domain.Entities;

public class Budget
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string AuthorId { get; set; } = default!;
    public string CurrencyId { get; set; } = default!;
}