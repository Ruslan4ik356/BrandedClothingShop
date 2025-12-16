using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BrandedClothingShop.Examples
{
    /// <summary>
    /// Example implementation showing how to use the ImageService
    /// for loading product images in WinForms applications.
    /// </summary>
    public static class ImageLoadingExamples
    {
        // ============================================================================
        // EXAMPLE 1: Loading Image in ProductDetailsForm
        // ============================================================================
        
        public static void Example_LoadImageInProductDetails(Product product, PictureBox pictureBox)
        {
            /*
             * This example shows how to load a product image in a detailed view.
             * The ImageService handles:
             * - Finding the image file
             * - Creating placeholder if image not found
             * - Safe file loading (no file locking)
             * - Error handling
             */
            
            try
            {
                // Simple way: Let ImageService handle everything
                ImageService.LoadImageToControl(pictureBox, product.ImagePath, useDefaultColor: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error");
            }
        }

        // ============================================================================
        // EXAMPLE 2: Auto-Building ImagePath in Product
        // ============================================================================

        public static void Example_AutoBuildImagePath()
        {
            /*
             * When creating products, automatically build the ImagePath
             * based on the product name. The service will search for
             * a matching image file in the Images folder.
             */
            
            var product = new Product
            {
                Id = 1,
                Name = "Nike Sport Premium Jacket",
                Brand = "Nike",
                Price = 3999.99m,
                // Automatically find image file matching "Nike Sport Premium Jacket"
                // Searches for: nike_sport_premium_jacket.jpg (and other extensions)
                ImagePath = ImageService.BuildImagePath("Nike Sport Premium Jacket"),
                Category = "Верхній одяг",
                Description = "Premium sports jacket from Nike",
            };

            // product.ImagePath will be "Images/nike_sport_premium_jacket.jpg"
            // or empty string if not found
        }

        // ============================================================================
        // EXAMPLE 3: Product List with Thumbnails
        // ============================================================================

        public static Panel Example_CreateProductCard(Product product)
        {
            /*
             * Create a product card with thumbnail image.
             * If image doesn't exist, a colored placeholder is shown.
             */
            
            var card = new Panel
            {
                Width = 270,
                Height = 420,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Image box
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 266,
                Height = 200,
                BackColor = Color.FromArgb(245, 245, 245),
                Dock = DockStyle.Top
            };

            // Load product image (with automatic placeholder fallback)
            ImageService.LoadImageToControl(pictureBox, product.ImagePath, useDefaultColor: true);
            card.Controls.Add(pictureBox);

            // Product info
            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 210)
            };
            card.Controls.Add(lblName);

            var lblPrice = new Label
            {
                Text = $"{product.Price:C}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 67, 54),
                AutoSize = true,
                Location = new Point(10, 240)
            };
            card.Controls.Add(lblPrice);

            return card;
        }

        // ============================================================================
        // EXAMPLE 4: Check Image Before Loading
        // ============================================================================

        public static void Example_CheckImageBeforeLoading(Product product, PictureBox pictureBox)
        {
            /*
             * Sometimes you want to check if an image exists before attempting to load.
             * This is useful for conditional logic or statistics.
             */
            
            if (ImageService.ImageExists(product.Name))
            {
                // Image found, load normally
                ImageService.LoadImageToControl(pictureBox, product.ImagePath);
                
                var label = new Label { Text = "✓ Image available" };
                // ... show image available indicator ...
            }
            else
            {
                // No image found, show placeholder with message
                ImageService.LoadImageToControl(pictureBox, "", useDefaultColor: true);
                
                var label = new Label { Text = "⚠ No image available" };
                // ... show no image indicator ...
            }
        }

        // ============================================================================
        // EXAMPLE 5: Get All Available Images
        // ============================================================================

        public static void Example_ListAllImages()
        {
            /*
             * Get a list of all available product images.
             * Useful for admin dashboards or verification.
             */
            
            var availableImages = ImageService.GetAvailableImages();
            
            var form = new Form
            {
                Text = "Available Product Images",
                Width = 400,
                Height = 300
            };

            var listBox = new ListBox { Dock = DockStyle.Fill };
            
            foreach (var imageName in availableImages)
            {
                listBox.Items.Add(imageName);
            }
            
            form.Controls.Add(listBox);
            form.ShowDialog();
        }

        // ============================================================================
        // EXAMPLE 6: Custom Placeholder Image
        // ============================================================================

        public static void Example_CustomPlaceholder()
        {
            /*
             * Create custom placeholder images with specific colors and text.
             */
            
            var placeholders = new Dictionary<string, Image>
            {
                {
                    "No Image",
                    ImageService.CreatePlaceholderImage(300, 300, "No Image Available", useColor: true)
                },
                {
                    "Not Found",
                    ImageService.CreatePlaceholderImage(300, 300, "Image Not Found", useColor: true)
                },
                {
                    "Error",
                    ImageService.CreatePlaceholderImage(300, 300, "Error Loading Image", useColor: true)
                },
                {
                    "Gray",
                    ImageService.CreatePlaceholderImage(300, 300, "Loading...", useColor: false)
                }
            };

            // Use placeholders
            var pictureBox = new PictureBox { Image = placeholders["No Image"] };
        }

        // ============================================================================
        // EXAMPLE 7: Batch Load Multiple Products
        // ============================================================================

        public static void Example_BatchLoadProducts(List<Product> products, Panel containerPanel)
        {
            /*
             * Load multiple product cards with images in a container.
             * Good for creating product grids/lists.
             */
            
            containerPanel.Controls.Clear();
            
            foreach (var product in products)
            {
                var card = Example_CreateProductCard(product);
                containerPanel.Controls.Add(card);
                
                // Optional: Show loading progress
                Application.DoEvents(); // Keep UI responsive
            }
        }

        // ============================================================================
        // EXAMPLE 8: Initialize on Application Startup
        // ============================================================================

        public static void Example_InitializeOnStartup()
        {
            /*
             * Call this in your Main() method or Form_Load.
             * Ensures the Images folder exists before trying to load any images.
             */
            
            try
            {
                // Create Images folder if it doesn't exist
                ImageService.EnsureImagesFolderExists();
                
                // Optional: List available images for debugging
                var images = ImageService.GetAvailableImages();
                System.Diagnostics.Debug.WriteLine($"Loaded {images.Count} product images");
                
                foreach (var image in images)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {image}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing image service: {ex.Message}", "Startup Error");
            }
        }

        // ============================================================================
        // EXAMPLE 9: Image Loading with Error Handling
        // ============================================================================

        public static void Example_RobustImageLoading(Product product, PictureBox pictureBox)
        {
            /*
             * Comprehensive error handling for image loading scenarios.
             */
            
            try
            {
                // First, check if image path is set
                if (string.IsNullOrWhiteSpace(product.ImagePath))
                {
                    // Try to auto-build path if not set
                    product.ImagePath = ImageService.BuildImagePath(product.Name);
                }

                // Load image with fallback
                ImageService.LoadImageToControl(pictureBox, product.ImagePath, useDefaultColor: true);
                
                // Optional: Log success
                System.Diagnostics.Debug.WriteLine($"Successfully loaded image for: {product.Name}");
            }
            catch (ArgumentException ex)
            {
                // Invalid path or corrupted image file
                System.Diagnostics.Debug.WriteLine($"Invalid image format: {ex.Message}");
                pictureBox.Image = ImageService.CreatePlaceholderImage(
                    pictureBox.Width, 
                    pictureBox.Height, 
                    "Invalid Image"
                );
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // File not found - already handled by ImageService
                System.Diagnostics.Debug.WriteLine($"Image file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Any other error
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading image: {ex.Message}");
                pictureBox.Image = ImageService.CreatePlaceholderImage(
                    pictureBox.Width, 
                    pictureBox.Height, 
                    "Error"
                );
            }
        }

        // ============================================================================
        // EXAMPLE 10: Performance-Conscious Loading
        // ============================================================================

        public static void Example_PerformanceOptimized(List<Product> visibleProducts, Panel containerPanel)
        {
            /*
             * Load only visible product images to optimize performance.
             * Useful for large product lists.
             */
            
            containerPanel.Controls.Clear();
            int loadedCount = 0;
            const int IMAGES_TO_LOAD = 10; // Load first 10 visible products

            foreach (var product in visibleProducts)
            {
                // Skip loading images for products not visible
                if (loadedCount >= IMAGES_TO_LOAD)
                {
                    break;
                }

                var card = Example_CreateProductCard(product);
                containerPanel.Controls.Add(card);
                loadedCount++;
                
                // Keep UI responsive
                Application.DoEvents();
            }

            // For remaining products, show lazy-load placeholders
            for (int i = loadedCount; i < visibleProducts.Count; i++)
            {
                var product = visibleProducts[i];
                var card = new Panel
                {
                    Width = 270,
                    Height = 420,
                    Margin = new Padding(10),
                    BackColor = Color.White
                };

                var pic = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 266,
                    Height = 200,
                    Image = ImageService.CreatePlaceholderImage(266, 200, "Click to load"),
                    Dock = DockStyle.Top
                };

                // Add click event to load image on demand
                pic.Click += (s, e) =>
                {
                    ImageService.LoadImageToControl(pic, product.ImagePath);
                };

                card.Controls.Add(pic);
                containerPanel.Controls.Add(card);
            }
        }
    }
}
