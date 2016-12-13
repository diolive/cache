namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
    public class CategoryWithDepthVM
    {
        public CategoryWithDepthVM(Category category)
        {
            this.Category = category;
            this.Depth = 0;
            Category current = category;
            while (current.ParentId.HasValue)
            {
                current = current.Parent;
                this.Depth++;
            }
        }

        public Category Category { get; }

        public int Depth { get; }
    }
}