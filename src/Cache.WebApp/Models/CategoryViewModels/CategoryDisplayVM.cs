namespace DioRed.Cache.WebApp.Models.CategoryViewModels;

public class CategoryDisplayVM : CategoryVM
{
    public List<CategoryDisplayVM> Children { get; set; } = [];
    public int TotalCost { get; set; }
}