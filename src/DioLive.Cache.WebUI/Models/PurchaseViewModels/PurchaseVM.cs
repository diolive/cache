using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels;

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