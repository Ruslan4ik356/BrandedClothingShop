# Image Loading Integration Guide

## Overview

This guide shows how to integrate the ImageService throughout your BrandedClothingShop application for complete image support.

## Architecture

```
┌─────────────────────────────────────┐
│  WinForms UI (Forms)                │
│  - ProductDetailsForm               │
│  - CatalogForm                      │
└──────────────┬──────────────────────┘
               │ Uses
               ▼
┌─────────────────────────────────────┐
│  ImageService                       │
│  - LoadImageToControl()             │
│  - BuildImagePath()                 │
│  - ImageExists()                    │
└──────────────┬──────────────────────┘
               │ Reads from
               ▼
┌─────────────────────────────────────┐
│  File System                        │
│  - Images/ folder                   │
│  - Product image files              │
└─────────────────────────────────────┘
```

## Integration Points

### 1. Application Startup (Program.cs)

```csharp
using BrandedClothingShop.Services;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Initialize image service on startup
        try
        {
            ImageService.EnsureImagesFolderExists();
            
            // Optional: Log available images
            var images = ImageService.GetAvailableImages();
            System.Diagnostics.Debug.WriteLine($"Loaded {images.Count} product images");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing image service: {ex.Message}");
        }

        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.Run(new LoginForm());
    }
}
```

### 2. Product Model (Models/Product.cs)

**Current Implementation:**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;  // ← Already has this!
    // ... other properties
}
```

**Usage in ProductService:**
```csharp
public static List<Product> GetAllProducts()
{
    return new List<Product>
    {
        new Product 
        { 
            Id = 1,
            Name = "Nike Sport Premium",
            Brand = "Nike",
            // Auto-build image path from product name
            ImagePath = ImageService.BuildImagePath("Nike Sport Premium"),
            // ... other properties
        },
        // ... more products
    };
}
```

### 3. ProductDetailsForm Integration

**Before (Hard-coded, file-locking issues):**
```csharp
private void LoadProductImage()
{
    try
    {
        string fullPath = @"D:\Projects\BrandedClothingShop\img\куртка nike.jpg";
        pictureBox.Image = Image.FromFile(fullPath); // ❌ Locks file!
    }
    catch
    {
        pictureBox.BackColor = Color.LightGray;
    }
}
```

**After (Using ImageService, proper error handling):**
```csharp
using BrandedClothingShop.Services;

private void InitializeComponent()
{
    // ... form setup ...
    
    var pictureBox = new PictureBox
    {
        SizeMode = PictureBoxSizeMode.StretchImage,
        Width = 420,
        Height = 470,
        BackColor = Color.FromArgb(245, 245, 245),
        Dock = DockStyle.Top
    };

    // Load product image with automatic fallback
    ImageService.LoadImageToControl(pictureBox, _product.ImagePath, useDefaultColor: true);
    
    this.Controls.Add(pictureBox);
}
```

### 4. CatalogForm Integration

**Product Card Creation:**
```csharp
private Panel CreateProductCard(Product product, Action addToCartAction)
{
    var card = new Panel
    {
        Width = 270,
        Height = 420,
        Margin = new Padding(10),
        BorderStyle = BorderStyle.None,
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

    // Use ImageService for safe image loading
    ImageService.LoadImageToControl(pictureBox, product.ImagePath, useDefaultColor: true);
    card.Controls.Add(pictureBox);

    // Product info (brand, name, price, etc.)
    // ... rest of card setup ...

    return card;
}
```

### 5. Admin/Product Management

**Add Product Dialog:**
```csharp
private void CreateProduct()
{
    var nameTextBox = new TextBox();
    var picturePreview = new PictureBox { Width = 200, Height = 200 };

    // Live preview of image matching product name
    nameTextBox.TextChanged += (s, e) =>
    {
        string imagePath = ImageService.BuildImagePath(nameTextBox.Text);
        ImageService.LoadImageToControl(picturePreview, imagePath);
    };
}
```

## Best Practices

### 1. Initialize on Startup ✓

```csharp
// Program.cs
ImageService.EnsureImagesFolderExists();
```

### 2. Auto-Build Image Paths ✓

```csharp
// ProductService.cs
ImagePath = ImageService.BuildImagePath(productName)
```

### 3. Use Service Method for Loading ✓

```csharp
// Forms
ImageService.LoadImageToControl(pictureBox, product.ImagePath);
```

### 4. Handle Missing Images ✓

```csharp
// The service shows placeholder automatically - no extra handling needed
ImageService.LoadImageToControl(pictureBox, imagePath);
// ✓ Shows image if found
// ✓ Shows colored placeholder if not found
```

### 5. Use Relative Paths ✓

```csharp
// Good: Relative path (works everywhere)
string path = "Images/product_name.jpg";

// Avoid: Absolute path (doesn't work on other machines)
string path = @"C:\Users\MyName\Projects\Images\product.jpg";
```

## Directory Structure

**Recommended project structure:**

```
BrandedClothingShop/
├── bin/
├── obj/
├── Images/                          ← Product images here
│   ├── nike_sport_premium.jpg
│   ├── adidas_classic_shirt.jpg
│   ├── puma_essentials_pants.png
│   ├── supreme_box_logo.jpg
│   ├── new_balance_574_shoes.jpg
│   ├── stussy_classic_cap.jpg
│   ├── carhartt_rugged_hoodie.jpg
│   ├── levis_501_jeans.png
│   ├── hugo_boss_shirt.jpg
│   ├── timberland_boots.jpg
│   ├── calvin_klein_sweatshirt.jpg
│   └── ray_ban_aviator_glasses.jpg
├── Forms/
├── Models/
├── Services/
│   └── ImageService.cs              ← Image loading service
├── Examples/
│   └── ImageLoadingExamples.cs
├── Data/
├── Program.cs
└── README.md
```

## Migration Checklist

If updating existing code to use ImageService:

### Step 1: Remove Old Image Code
```csharp
// ❌ Remove this
string fullPath = Path.Combine(_imgFolderPath, fileName);
if (File.Exists(fullPath))
{
    pic.Image = Image.FromFile(fullPath);
}
```

### Step 2: Add ImageService Usage
```csharp
// ✅ Replace with this
ImageService.LoadImageToControl(pic, product.ImagePath);
```

### Step 3: Update ProductService
```csharp
// ✅ Add auto image path
ImagePath = ImageService.BuildImagePath(product.Name)
```

### Step 4: Test
```csharp
// ✅ Verify images load correctly
var images = ImageService.GetAvailableImages();
Assert.IsTrue(images.Count > 0);
```

## Common Scenarios

### Scenario 1: Product List with Lazy Loading

```csharp
public void LoadProductsLazy(List<Product> products, FlowLayoutPanel container)
{
    // Load only first 10 images for performance
    int count = 0;
    foreach (var product in products)
    {
        if (count >= 10) break;
        
        var card = CreateProductCard(product);
        container.Controls.Add(card);
        count++;
        Application.DoEvents();
    }
}
```

### Scenario 2: Product Search with Image Preview

```csharp
private void SearchProducts(string query)
{
    var results = ProductService.SearchProducts(query);
    
    var resultForm = new Form { Title = "Search Results" };
    var container = new FlowLayoutPanel { Dock = DockStyle.Fill };
    
    foreach (var product in results)
    {
        var card = CreateProductCard(product);
        container.Controls.Add(card);
    }
    
    resultForm.Controls.Add(container);
    resultForm.ShowDialog();
}
```

### Scenario 3: Product Comparison View

```csharp
private void ShowComparisonView(Product product1, Product product2)
{
    var pic1 = new PictureBox { Width = 300, Height = 300 };
    var pic2 = new PictureBox { Width = 300, Height = 300 };
    
    // Load both images
    ImageService.LoadImageToControl(pic1, product1.ImagePath);
    ImageService.LoadImageToControl(pic2, product2.ImagePath);
    
    // Display side by side
    // ... add to form
}
```

### Scenario 4: Image Gallery

```csharp
private void ShowImageGallery(Product product)
{
    var form = new Form { Text = "Product Images" };
    var pictureBox = new PictureBox { Dock = DockStyle.Fill };
    
    // Load main image
    ImageService.LoadImageToControl(pictureBox, product.ImagePath);
    
    // Could extend to show multiple images per product
    form.Controls.Add(pictureBox);
    form.ShowDialog();
}
```

## Error Handling Strategy

### User-Friendly Error Handling

```csharp
private void SafeLoadImage(Product product, PictureBox pictureBox)
{
    try
    {
        // Step 1: Build path if not set
        if (string.IsNullOrEmpty(product.ImagePath))
        {
            product.ImagePath = ImageService.BuildImagePath(product.Name);
        }
        
        // Step 2: Check if image exists
        if (!string.IsNullOrEmpty(product.ImagePath))
        {
            ImageService.LoadImageToControl(pictureBox, product.ImagePath);
        }
        else
        {
            // No image found
            pictureBox.Image = ImageService.CreatePlaceholderImage(
                pictureBox.Width,
                pictureBox.Height,
                "No Image Available"
            );
        }
    }
    catch (Exception ex)
    {
        // Log error for debugging
        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
        
        // Show placeholder to user
        pictureBox.Image = ImageService.CreatePlaceholderImage(
            pictureBox.Width,
            pictureBox.Height,
            "Error Loading Image"
        );
    }
}
```

## Performance Optimization

### Load Only Visible Images

```csharp
private void LoadVisibleImages(FlowLayoutPanel panel, List<Product> products)
{
    var visibleControls = panel.Controls.OfType<Control>()
        .Where(c => c.ClientRectangle.IntersectsWith(panel.DisplayRectangle))
        .ToList();
    
    foreach (Control control in visibleControls)
    {
        // Load image for visible control only
        if (control is Panel card && card.Tag is Product product)
        {
            LoadProductImage(product, card);
        }
    }
}
```

### Image Caching (Optional)

```csharp
private static Dictionary<string, Image> _imageCache = new();

public static Image GetCachedImage(string imagePath)
{
    if (_imageCache.ContainsKey(imagePath))
        return _imageCache[imagePath];
    
    var image = ImageService.LoadImage(imagePath);
    _imageCache[imagePath] = image;
    return image;
}
```

## Testing

### Unit Test Example

```csharp
[TestClass]
public class ImageServiceTests
{
    [TestMethod]
    public void BuildImagePath_ValidProductName_ReturnsPath()
    {
        // Arrange
        string productName = "Nike Jacket";
        
        // Act
        string result = ImageService.BuildImagePath(productName);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.StartsWith("Images/"));
    }
    
    [TestMethod]
    public void ImageExists_ValidProduct_ReturnsTrue()
    {
        // Arrange
        ImageService.EnsureImagesFolderExists();
        
        // Act
        bool result = ImageService.ImageExists("Nike Jacket");
        
        // Assert
        Assert.IsTrue(result);
    }
}
```

## Summary

✅ **ImageService provides:**
- Automatic image path building from product names
- Safe, non-file-locking image loading
- Beautiful placeholder images for missing files
- WinForms-ready methods
- Complete error handling
- Portable code (relative paths)

✅ **To integrate:**
1. Call `ImageService.EnsureImagesFolderExists()` in Program.cs
2. Use `ImageService.BuildImagePath()` in ProductService
3. Replace image loading code with `ImageService.LoadImageToControl()`
4. Place product images in the `Images/` folder

✅ **Result:**
- Robust image handling throughout the application
- Professional placeholder images
- No file locking issues
- Easy to maintain and extend
