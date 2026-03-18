namespace DioRed.Cache.Domain.Entities;

public class BudgetSlim
{
    public Guid Id { get; set; }
    public string Currency { get; set; } = default!;
}