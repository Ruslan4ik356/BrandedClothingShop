using BrandedClothingShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Services
{
    public static class ReviewService
    {
        private static readonly string _filePath = Path.Combine(Application.StartupPath, "Data", "reviews.json");

        static ReviewService()
        {
            string dir = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public static List<Review> LoadReviews()
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Review>>(json) ?? new List<Review>();
        }

        public static void SaveReviews(List<Review> reviews)
        {
            string json = JsonConvert.SerializeObject(reviews, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static void AddReview(int productId, string userEmail, string userName, int rating, string comment)
        {
            var reviews = LoadReviews();
            reviews.Add(new Review
            {
                Id = reviews.Count > 0 ? reviews.Max(r => r.Id) + 1 : 1,
                ProductId = productId,
                UserEmail = userEmail,
                UserName = userName,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            });
            SaveReviews(reviews);
        }

        public static List<Review> GetProductReviews(int productId)
        {
            var reviews = LoadReviews();
            return reviews.Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedDate).ToList();
        }

        public static double GetAverageRating(int productId)
        {
            var reviews = GetProductReviews(productId);
            if (reviews.Count == 0) return 5;
            return Math.Round(reviews.Average(r => r.Rating), 1);
        }
    }
}
