using BrandedClothingShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Services
{
    public static class WishlistService
    {
        private static readonly string _filePath = Path.Combine(Application.StartupPath, "Data", "wishlist.json");

        static WishlistService()
        {
            string dir = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public static List<Wishlist> LoadWishlist()
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Wishlist>>(json) ?? new List<Wishlist>();
        }

        public static void SaveWishlist(List<Wishlist> items)
        {
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static void AddToWishlist(string userEmail, int productId)
        {
            var wishlist = LoadWishlist();
            if (!wishlist.Any(w => w.UserEmail == userEmail && w.ProductId == productId))
            {
                wishlist.Add(new Wishlist
                {
                    Id = wishlist.Count > 0 ? wishlist.Max(w => w.Id) + 1 : 1,
                    UserEmail = userEmail,
                    ProductId = productId,
                    AddedDate = DateTime.Now
                });
                SaveWishlist(wishlist);
            }
        }

        public static void RemoveFromWishlist(string userEmail, int productId)
        {
            var wishlist = LoadWishlist();
            wishlist.RemoveAll(w => w.UserEmail == userEmail && w.ProductId == productId);
            SaveWishlist(wishlist);
        }

        public static List<int> GetUserWishlist(string userEmail)
        {
            var wishlist = LoadWishlist();
            return wishlist.Where(w => w.UserEmail == userEmail).Select(w => w.ProductId).ToList();
        }

        public static bool IsInWishlist(string userEmail, int productId)
        {
            var wishlist = LoadWishlist();
            return wishlist.Any(w => w.UserEmail == userEmail && w.ProductId == productId);
        }
    }
}