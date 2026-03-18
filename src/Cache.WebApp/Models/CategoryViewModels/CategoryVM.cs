using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.WebApp.Models.CategoryViewModels;

public class CategoryVM
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }

    public static CategoryVM Build(Category category)
    {
        return new CategoryVM
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color.ToString("X6")
        };
    }
}