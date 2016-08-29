namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
    public class CategoryDisplayVM : CategoryVM
    {
        public CategoryDisplayVM[] Children { get; set; }

        public int TotalCost { get; set; }
    }
}