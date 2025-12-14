using BrandedClothingShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Services
{
    public static class OrderService
    {
        private static readonly string _filePath = Path.Combine(Application.StartupPath, "Data", "orders.json");
        private static int _nextOrderId = 1;

        // Тарифи доставки
        public static Dictionary<string, decimal> ShippingRates = new()
        {
            { "Standard", 49.99m },     // Стандартная - 2-3 дня
            { "Express", 99.99m },      // Экспресс - 1 день
            { "Pickup", 0m }            // Самовывоз - бесплатно
        };

        // Дополнительные расходы по размеру заказа
        public static decimal GetShippingCost(decimal subtotal, string shippingMethod)
        {
            if (!ShippingRates.ContainsKey(shippingMethod))
                shippingMethod = "Standard";

            decimal baseCost = ShippingRates[shippingMethod];
            
            // Бесплатная доставка при заказе более 500 гривень (кроме срочной)
            if (subtotal >= 500 && shippingMethod != "Express")
                return 0;

            return baseCost;
        }

        static OrderService()
        {
            string dir = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
            else
                LoadMaxOrderId();
        }

        private static void LoadMaxOrderId()
        {
            var orders = LoadOrders();
            if (orders.Count > 0)
                _nextOrderId = orders.Max(o => o.Id) + 1;
        }

        public static List<Order> LoadOrders()
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }

        public static void SaveOrders(List<Order> orders)
        {
            string json = JsonConvert.SerializeObject(orders, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static Order? CreateOrder(string userEmail, List<CartItem> items, 
            string deliveryAddress = "", string deliveryCity = "", string deliveryPostalCode = "",
            string deliveryPhone = "", string deliveryName = "", string shippingMethod = "Standard")
        {
            if (items == null || items.Count == 0)
                return null;

            decimal subTotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal shippingCost = GetShippingCost(subTotal, shippingMethod);
            decimal totalPrice = subTotal + shippingCost;

            var order = new Order
            {
                Id = _nextOrderId++,
                UserEmail = userEmail,
                Items = new List<CartItem>(items),
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                ShippingMethod = shippingMethod,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                Status = "Обробляється",
                DeliveryAddress = deliveryAddress,
                DeliveryCity = deliveryCity,
                DeliveryPostalCode = deliveryPostalCode,
                DeliveryPhone = deliveryPhone,
                DeliveryName = deliveryName,
                DeliveryCountry = "Ukraine"
            };

            var orders = LoadOrders();
            orders.Add(order);
            SaveOrders(orders);

            return order;
        }

        public static List<Order> GetUserOrders(string userEmail)
        {
            var orders = LoadOrders();
            return orders.Where(o => o.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static void UpdateOrderStatus(int orderId, string status)
        {
            var orders = LoadOrders();
            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
                SaveOrders(orders);
            }
        }

        public static void DeleteOrder(int orderId)
        {
            var orders = LoadOrders();
            orders.RemoveAll(o => o.Id == orderId);
            SaveOrders(orders);
        }
    }
}
