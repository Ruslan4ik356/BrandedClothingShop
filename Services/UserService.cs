using BrandedClothingShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Services
{
    public static class UserService
    {
        private static readonly string _filePath = Path.Combine(Application.StartupPath, "Data", "users.json");

        static UserService()
        {
            string dir = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public static List<User> LoadUsers()
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }

        public static void SaveUsers(List<User> users)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static bool RegisterUser(User user)
        {
            var users = LoadUsers();
            if (users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                return false;

            users.Add(user);
            SaveUsers(users);
            return true;
        }

        public static User? Authenticate(string email, string password)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);
        }

        public static void UpdateUser(User user)
        {
            var users = LoadUsers();
            var existingUser = users.FirstOrDefault(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.City = user.City;
                existingUser.PostalCode = user.PostalCode;
                existingUser.Country = user.Country;
                SaveUsers(users);
            }
        }
    }
}