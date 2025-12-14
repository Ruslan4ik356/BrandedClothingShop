namespace BrandedClothingShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; } // Для скидок
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Rating { get; set; } = 5;
        public int ReviewCount { get; set; } = 0;
        public string AvailableSizes { get; set; } = "XS,S,M,L,XL,XXL";
        public int Stock { get; set; } = 100; // Остаток товара
        public bool IsNew { get; set; } = false;
        public bool IsDiscount { get; set; } = false;
        public List<string> Colors { get; set; } = new List<string> { "Black", "White", "Gray" };
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}