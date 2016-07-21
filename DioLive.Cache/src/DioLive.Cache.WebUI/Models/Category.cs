namespace DioLive.Cache.WebUI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}