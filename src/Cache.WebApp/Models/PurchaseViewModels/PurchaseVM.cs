using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain;
using DioRed.Cache.WebApp.Models.CategoryViewModels;

namespace DioRed.Cache.WebApp.Models.PurchaseViewModels;

public class PurchaseVM
{
    public static PurchaseVM Build(
        Purchase purchase,
        Category category,
        string currency
    )
    {
        return new PurchaseVM
        {
            Id = purchase.Id,
            Name = purchase.Name,
            Category = CategoryVM.Build(category),
            Date = purchase.Date,
            CostValue = purchase.Cost,
            Cost = string.Format(Constants.CostDisplayFormat, purchase.Cost, currency),
            Shop = purchase.Shop,
            Comments = purchase.Comments
        };
    }

    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public required CategoryVM Category { get; set; }

    [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
    public required DateTime Date { get; set; }

    public required decimal CostValue { get; set; }

    public required string Cost { get; set; }

    [DisplayFormat(NullDisplayText = "N/A")]
    public string? Shop { get; set; }

    public string? Comments { get; set; }
}