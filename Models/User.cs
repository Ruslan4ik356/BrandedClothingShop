namespace BrandedClothingShop.Models
{
    public class User
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Ukraine";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<int> WishlistProductIds { get; set; } = new List<int>();
        public List<int> ViewedProductIds { get; set; } = new List<int>();
    }
}