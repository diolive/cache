using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain;

namespace DioRed.Cache.WebApp.Models.PurchaseViewModels;

public class CreatePurchaseVM
{
    [Required]
    [StringLength(300)]
    public required string Name { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
    [DataType(DataType.Text)]
    public DateTime Date { get; set; }

    [Required]
    public decimal? Cost { get; set; }

    public string? Shop { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Comments { get; set; }

    public int? PlanId { get; set; }
}