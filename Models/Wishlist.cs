namespace BrandedClothingShop.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
