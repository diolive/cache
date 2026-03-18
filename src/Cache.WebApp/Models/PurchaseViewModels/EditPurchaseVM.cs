using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Domain;

namespace DioRed.Cache.WebApp.Models.PurchaseViewModels;

public class EditPurchaseVM
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(300)]
    public required string Name { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
    [DataType(DataType.Text)]
    public required DateTime Date { get; set; }

    public decimal Cost { get; set; }

    public string? Shop { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Comments { get; set; }

    public required string AuthorId { get; set; }

    public required string AuthorName { get; set; }

    public string? LastEditorId { get; set; }

    public string? LastEditorName { get; set; }

    public static EditPurchaseVM Build(Purchase purchase, string authorName, string? lastEditorName)
    {
        return new EditPurchaseVM
        {
            Id = purchase.Id,
            Name = purchase.Name,
            CategoryId = purchase.CategoryId,
            Date = purchase.Date,
            Cost = purchase.Cost,
            Shop = purchase.Shop,
            Comments = purchase.Comments,
            AuthorId = purchase.AuthorId,
            AuthorName = authorName,
            LastEditorId = purchase.LastEditorId,
            LastEditorName = lastEditorName
        };
    }
}