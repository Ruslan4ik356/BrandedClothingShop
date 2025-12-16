using BrandedClothingShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrandedClothingShop.Services
{
    public static class ProductService
    {
        public static List<Product> GetAllProducts()
        {
            return new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Куртка Nike Sport Premium", 
                    Brand = "Nike", 
                    Price = 3999.99m,
                    OriginalPrice = 4999.99m,
                    Category = "Верхній одяг",
                    Description = "Комфортна спортивна куртка від Nike з вітрозахисною тканиною. Ідеальна для тренувань та повсякденного носіння.",
                    ImagePath = "img/куртка nike.jpg",
                    Rating = 5,
                    ReviewCount = 245,
                    Stock = 50,
                    IsNew = true,
                    IsDiscount = true,
                    Colors = new List<string> { "Black", "White", "Red", "Blue" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Футболка Adidas Classic", 
                    Brand = "Adidas", 
                    Price = 799.50m,
                    OriginalPrice = 899.50m,
                    Category = "Одяг",
                    Description = "Класична біла футболка з логотипом Adidas. Виконана з 100% бавовни, дихаюча та комфортна.",
                    ImagePath = "img/футболка adidas.jpeg",
                    Rating = 4,
                    ReviewCount = 128,
                    Stock = 100,
                    IsDiscount = true,
                    Colors = new List<string> { "White", "Black", "Gray", "Navy" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 3, 
                    Name = "Штани Puma Essentials", 
                    Brand = "Puma", 
                    Price = 1499.00m,
                    OriginalPrice = 1799.00m,
                    Category = "Нижній одяг",
                    Description = "Універсальні спортивні штани для активного образу життя. Легкі, зручні та стильні.",
                    ImagePath = "img/штани Puma.jpg",
                    Rating = 4,
                    ReviewCount = 89,
                    Stock = 75,
                    IsDiscount = true,
                    Colors = new List<string> { "Black", "Gray", "Navy" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 4, 
                    Name = "Худі Supreme Box Logo", 
                    Brand = "Supreme", 
                    Price = 8500.00m,
                    Category = "Верхній одяг",
                    Description = "Культова модель худі з характерним лого Supreme. Преміум якість, обмежена кількість випусків.",
                    ImagePath = "img/худи Supreme.jpg",
                    Rating = 5,
                    ReviewCount = 312,
                    Stock = 20,
                    IsNew = true,
                    Colors = new List<string> { "Red", "Black", "White" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 5, 
                    Name = "Кросівки New Balance 574", 
                    Brand = "New Balance", 
                    Price = 2999.99m,
                    OriginalPrice = 3299.99m,
                    Category = "Взуття",
                    Description = "Культові кросівки для комфортної ходьби та бігу. Оригінальний дизайн з прекрасною амортизацією.",
                    ImagePath = "img/кроссовки new balans.jpg",
                    Rating = 5,
                    ReviewCount = 567,
                    Stock = 60,
                    IsDiscount = true,
                    Colors = new List<string> { "Gray", "White", "Black", "Navy" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 6, 
                    Name = "Кепка Stüssy Classic", 
                    Brand = "Stüssy", 
                    Price = 599.00m,
                    OriginalPrice = 799.00m,
                    Category = "Аксесуари",
                    Description = "Стильна кепка від легендарного бренду Stüssy. Перфектна для будь-якої вікової категорії.",
                    ImagePath = "img/кепка stussy.jpg",
                    Rating = 4,
                    ReviewCount = 145,
                    Stock = 150,
                    IsDiscount = true,
                    Colors = new List<string> { "Black", "White", "Beige" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 7, 
                    Name = "Толстовка Carhartt Rugged", 
                    Brand = "Carhartt", 
                    Price = 2299.00m,
                    OriginalPrice = 2499.00m,
                    Category = "Верхній одяг",
                    Description = "Міцна і надійна толстовка від класичного американського бренду. Ідеальна для холодної погоди.",
                    ImagePath = "img/свитшот carhartt.jpg",
                    Rating = 4,
                    ReviewCount = 203,
                    Stock = 45,
                    IsDiscount = true,
                    Colors = new List<string> { "Brown", "Black", "Gray" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 8, 
                    Name = "Джинси Levi's 501 Original", 
                    Brand = "Levi's", 
                    Price = 2599.99m,
                    OriginalPrice = 2899.99m,
                    Category = "Нижній одяг",
                    Description = "Класичні джинси, які не вийдуть з моди. Якість та комфорт, що довірили мільйони людей.",
                    ImagePath = "img/джинси levis.png",
                    Rating = 5,
                    ReviewCount = 456,
                    Stock = 80,
                    IsDiscount = true,
                    Colors = new List<string> { "Blue", "Black", "Gray" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 9, 
                    Name = "Рубашка Hugo Boss", 
                    Brand = "Hugo Boss", 
                    Price = 2199.00m,
                    Category = "Одяг",
                    Description = "Елегантна класична рубашка від відомого німецького дизайнера. Ідеальна для офісу та святкових подій.",
                    ImagePath = "img/рубашка hugo boss.jpg",
                    Rating = 5,
                    ReviewCount = 178,
                    Stock = 35,
                    IsNew = true,
                    Colors = new List<string> { "White", "Blue", "Black", "Pink" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 10, 
                    Name = "Спортивні черевики Timberland", 
                    Brand = "Timberland", 
                    Price = 4299.00m,
                    OriginalPrice = 5499.00m,
                    Category = "Взуття",
                    Description = "Міцні та надійні черевики для активного відпочинку. Водонепроникні та комфортні.",
                    ImagePath = "img/timberland.jpg",
                    Rating = 5,
                    ReviewCount = 289,
                    Stock = 40,
                    IsDiscount = true,
                    Colors = new List<string> { "Brown", "Black", "Tan" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 11, 
                    Name = "Світшот Calvin Klein", 
                    Brand = "Calvin Klein", 
                    Price = 1699.00m,
                    Category = "Одяг",
                    Description = "Стильний мінімалістичний світшот від Calvin Klein. Висока якість та комфорт.",
                    ImagePath = "img/свитшот calvin klein.jpg",
                    Rating = 4,
                    ReviewCount = 167,
                    Stock = 55,
                    Colors = new List<string> { "White", "Black", "Gray", "Navy" },
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 12, 
                    Name = "Очки Ray-Ban Aviator", 
                    Brand = "Ray-Ban", 
                    Price = 3999.00m,
                    Category = "Аксесуари",
                    Description = "Культові сонячні окуляри з рефлективним об'єктивом. Класичний дизайн та надійна якість.",
                    ImagePath = "img/очки ray ban.jpg",
                    Rating = 5,
                    ReviewCount = 523,
                    Stock = 70,
                    IsNew = true,
                    Colors = new List<string> { "Gold", "Silver", "Black" },
                    CreatedDate = DateTime.Now
                }
            };
        }

        public static List<Product> SearchProducts(string query)
        {
            var products = GetAllProducts();
            var searchQuery = query.ToLower();
            return products
                .Where(p => p.Name.ToLower().Contains(searchQuery) || 
                           p.Brand.ToLower().Contains(searchQuery) ||
                           p.Description.ToLower().Contains(searchQuery))
                .ToList();
        }

        public static Product? GetProductById(int id)
        {
            return GetAllProducts().FirstOrDefault(p => p.Id == id);
        }

        public static List<Product> FilterByPrice(decimal minPrice, decimal maxPrice)
        {
            return GetAllProducts()
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToList();
        }

        public static List<Product> FilterByBrand(string brand)
        {
            return GetAllProducts()
                .Where(p => p.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public static List<Product> FilterByCategory(string category)
        {
            return GetAllProducts()
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public static List<Product> GetNewProducts()
        {
            return GetAllProducts().Where(p => p.IsNew).ToList();
        }

        public static List<Product> GetDiscountedProducts()
        {
            return GetAllProducts().Where(p => p.IsDiscount).ToList();
        }

        public static List<Product> GetTopRatedProducts()
        {
            return GetAllProducts().OrderByDescending(p => p.Rating).Take(6).ToList();
        }

        public static List<string> GetAllBrands()
        {
            return GetAllProducts()
                .Select(p => p.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToList();
        }

        public static List<string> GetAllCategories()
        {
            return GetAllProducts()
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public static decimal GetDiscount(Product product)
        {
            if (product.OriginalPrice <= 0 || product.OriginalPrice <= product.Price)
                return 0;
            return Math.Round(((product.OriginalPrice - product.Price) / product.OriginalPrice) * 100, 0);
        }
    }
}
