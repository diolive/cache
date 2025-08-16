namespace DioLive.Cache.WebUI.Models.CategoryViewModels;

public class CategoryDisplayVM : CategoryVM
{
    public List<CategoryDisplayVM> Children { get; set; } = [];
    public int TotalCost { get; set; }
}