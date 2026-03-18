using System.ComponentModel.DataAnnotations;

namespace DioRed.Cache.WebApp.Models.CategoryViewModels;

public class CreateCategoryVM
{
    [Required]
    [StringLength(300)]
    public required string Name { get; set; }
}