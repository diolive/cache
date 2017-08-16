namespace DioLive.BlackMint.Entities
{
    public class PurchaseItem
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}