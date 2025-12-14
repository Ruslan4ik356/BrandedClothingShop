using System;
using System.Collections.Generic;

namespace BrandedClothingShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
        public decimal SubTotal { get; set; }  // Сумма товаров без доставки
        public decimal ShippingCost { get; set; }  // Стоимость доставки
        public string ShippingMethod { get; set; } = "Standard";  // "Standard", "Express", "Pickup"
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; } = "Обробляється";  // "Обробляється", "Відправлено", "Доставлено", "Скасовано"
        
        // Адрес доставки
        public string DeliveryAddress { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryPostalCode { get; set; } = string.Empty;
        public string DeliveryCountry { get; set; } = "Ukraine";
        public string DeliveryPhone { get; set; } = string.Empty;
        public string DeliveryName { get; set; } = string.Empty;  // Прізвище та ім'я отримувача
    }
}
