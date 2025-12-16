using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BrandedClothingShop.Services
{
    /// <summary>
    /// Service for handling product image loading and management.
    /// Handles loading images from the "Images" folder with fallback to placeholder images.
    /// </summary>
    public static class ImageService
    {
        // Supported image extensions
        private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

        /// <summary>
        /// Gets the path to the Images folder.
        /// </summary>
        public static string GetImagesFolderPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        }

        /// <summary>
        /// Builds the ImagePath property value for a product based on its name.
        /// </summary>
        /// <param name="productName">The product name</param>
        /// <returns>Relative path to the image file (e.g., "Images/TShirt_Black.jpg")</returns>
        public static string BuildImagePath(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return string.Empty;

            // Try to find an image file matching the product name
            string imageFolderPath = GetImagesFolderPath();
            
            if (!Directory.Exists(imageFolderPath))
                return string.Empty;

            // Normalize product name for file matching
            string normalizedName = NormalizeProductName(productName);

            // Search for image files with matching name
            try
            {
                foreach (var ext in SupportedExtensions)
                {
                    string fileName = normalizedName + ext;
                    string filePath = Path.Combine(imageFolderPath, fileName);
                    
                    if (File.Exists(filePath))
                    {
                        return Path.Combine("Images", fileName);
                    }
                }

                // If exact match not found, try partial matching
                var files = Directory.GetFiles(imageFolderPath);
                var matchingFile = files.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f)
                        .IndexOf(normalizedName, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchingFile != null)
                {
                    return Path.Combine("Images", Path.GetFileName(matchingFile));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error building image path: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Loads an image from the specified path and returns it as a Bitmap.
        /// If the image cannot be loaded, returns a placeholder image.
        /// </summary>
        /// <param name="imagePath">Relative or absolute path to the image</param>
        /// <param name="width">Width of the image (for placeholder)</param>
        /// <param name="height">Height of the image (for placeholder)</param>
        /// <returns>Image object (either loaded image or placeholder)</returns>
        public static Image LoadImage(string imagePath, int width = 300, int height = 300)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                    return CreatePlaceholderImage(width, height, "No Image");

                // Convert relative path to absolute if needed
                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                }

                if (File.Exists(fullPath))
                {
                    // Load image without locking the file
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return Image.FromStream(fs);
                    }
                }
                else
                {
                    return CreatePlaceholderImage(width, height, "Image Not Found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                return CreatePlaceholderImage(width, height, "Error Loading Image");
            }
        }

        /// <summary>
        /// Safely loads an image into a PictureBox with error handling.
        /// </summary>
        /// <param name="pictureBox">The PictureBox control to load the image into</param>
        /// <param name="imagePath">Path to the image file</param>
        /// <param name="useDefaultColor">Whether to use a colored placeholder</param>
        public static void LoadImageToControl(System.Windows.Forms.PictureBox pictureBox, string imagePath, bool useDefaultColor = true)
        {
            try
            {
                if (pictureBox == null)
                    return;

                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    pictureBox.Image = CreatePlaceholderImage(pictureBox.Width, pictureBox.Height, "No Image");
                    return;
                }

                // Convert relative path to absolute if needed
                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                }

                if (File.Exists(fullPath))
                {
                    // Create a copy of the image to avoid file locking
                    using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    pictureBox.Image = CreatePlaceholderImage(
                        pictureBox.Width, 
                        pictureBox.Height, 
                        "Image Not Found",
                        useDefaultColor
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image to control: {ex.Message}");
                pictureBox.Image = CreatePlaceholderImage(
                    pictureBox.Width,
                    pictureBox.Height,
                    "Error Loading",
                    useDefaultColor
                );
            }
        }

        /// <summary>
        /// Creates a placeholder image with text and optional colored background.
        /// </summary>
        /// <param name="width">Width of the placeholder</param>
        /// <param name="height">Height of the placeholder</param>
        /// <param name="text">Text to display on the placeholder</param>
        /// <param name="useColor">Whether to use a colored background (cycling through colors)</param>
        /// <returns>Placeholder image as Bitmap</returns>
        public static Bitmap CreatePlaceholderImage(int width, int height, string text = "No Image", bool useColor = true)
        {
            var bitmap = new Bitmap(width, height);
            
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Background color
                Color bgColor = useColor ? GetPlaceholderColor(text) : Color.FromArgb(200, 200, 200);
                graphics.Clear(bgColor);

                // Text rendering
                var font = new Font("Segoe UI", Math.Min(width, height) / 10, FontStyle.Bold);
                var brush = new SolidBrush(Color.White);
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(text, font, brush, new Rectangle(0, 0, width, height), stringFormat);

                font.Dispose();
                brush.Dispose();
            }

            return bitmap;
        }

        /// <summary>
        /// Checks if an image file exists for the given product name.
        /// </summary>
        /// <param name="productName">The product name</param>
        /// <returns>True if an image file exists, false otherwise</returns>
        public static bool ImageExists(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return false;

            string imageFolderPath = GetImagesFolderPath();
            if (!Directory.Exists(imageFolderPath))
                return false;

            string normalizedName = NormalizeProductName(productName);

            try
            {
                foreach (var ext in SupportedExtensions)
                {
                    string filePath = Path.Combine(imageFolderPath, normalizedName + ext);
                    if (File.Exists(filePath))
                        return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Gets all available product images from the Images folder.
        /// </summary>
        /// <returns>List of image file names</returns>
        public static List<string> GetAvailableImages()
        {
            var images = new List<string>();
            string imageFolderPath = GetImagesFolderPath();

            if (!Directory.Exists(imageFolderPath))
                return images;

            try
            {
                var files = Directory.GetFiles(imageFolderPath);
                foreach (var file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (SupportedExtensions.Contains(ext))
                    {
                        images.Add(Path.GetFileName(file));
                    }
                }
            }
            catch { }

            return images;
        }

        /// <summary>
        /// Normalizes a product name for use in file matching.
        /// Removes special characters and extra spaces.
        /// </summary>
        private static string NormalizeProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Convert to lowercase and trim
            var result = name.ToLower().Trim();

            // Remove or replace special characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            // Replace spaces with underscores
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", "_");

            // Replace multiple underscores with single
            result = System.Text.RegularExpressions.Regex.Replace(result, "_+", "_");

            return result;
        }

        /// <summary>
        /// Gets a color for placeholder image based on hash of the text.
        /// </summary>
        private static Color GetPlaceholderColor(string text)
        {
            Color[] colors = new[]
            {
                Color.FromArgb(33, 150, 243),    // Blue
                Color.FromArgb(76, 175, 80),    // Green
                Color.FromArgb(244, 67, 54),    // Red
                Color.FromArgb(233, 30, 99),    // Pink
                Color.FromArgb(255, 152, 0),    // Orange
                Color.FromArgb(156, 39, 176),   // Purple
                Color.FromArgb(63, 81, 181),    // Indigo
                Color.FromArgb(0, 150, 136),    // Teal
                Color.FromArgb(255, 193, 7),    // Amber
                Color.FromArgb(139, 69, 19),    // Brown
                Color.FromArgb(96, 125, 139),   // Blue Grey
                Color.FromArgb(0, 0, 0)         // Black
            };

            int hash = text.GetHashCode();
            int index = Math.Abs(hash) % colors.Length;
            return colors[index];
        }

        /// <summary>
        /// Ensures the Images folder exists, creating it if necessary.
        /// </summary>
        public static void EnsureImagesFolderExists()
        {
            string imagePath = GetImagesFolderPath();
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
        }
    }
}
