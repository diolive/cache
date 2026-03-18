using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain;

namespace DioRed.Cache.WebApp.Models.PurchaseViewModels;

public class DeletePurchaseVM
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
    public required DateTime Date { get; set; }

    public required string Cost { get; set; }

    [DisplayFormat(NullDisplayText = "N/A")]
    public string? Shop { get; set; }

    public string? Comments { get; set; }

    public static DeletePurchaseVM Build(
        Purchase purchase,
        string currency
    )
    {
        return new DeletePurchaseVM
        {
            Id = purchase.Id,
            Name = purchase.Name,
            Date = purchase.Date,
            Cost = string.Format(Constants.CostDisplayFormat, purchase.Cost, currency),
            Shop = purchase.Shop,
            Comments = purchase.Comments
        };
    }
}