using System.ComponentModel.DataAnnotations;

namespace DioRed.Cache.WebApp.Models.CategoryViewModels;

public class UpdateCategoryVM
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Color { get; set; }

    public int? ParentId { get; set; }
}