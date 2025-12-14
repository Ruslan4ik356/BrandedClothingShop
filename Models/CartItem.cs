using BrandedClothingShop.Models;

namespace BrandedClothingShop.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public string Size { get; set; } = "M"; // Размер по умолчанию
    }
}