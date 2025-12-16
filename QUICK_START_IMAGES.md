# Quick Start Guide: Image Loading

## 5-Minute Setup

### Step 1: Ensure Images Folder Exists
```csharp
// In Program.cs or Form_Load()
ImageService.EnsureImagesFolderExists();
```

### Step 2: Place Product Images
Add image files to the `Images` folder following these naming patterns:

```
Images/
├── tshirt_black.jpg
├── jacket_nike.png
├── pants_adidas.jpg
└── shoes_nike.jpeg
```

### Step 3: Update Product ImagePath
```csharp
var product = new Product
{
    Name = "Nike Jacket",
    // Auto-find image file
    ImagePath = ImageService.BuildImagePath("Nike Jacket"),
    // ... other properties
};
```

### Step 4: Display Image in UI
```csharp
// In your form
ImageService.LoadImageToControl(pictureBox, product.ImagePath);
```

## Common File Name Examples

| Product Name | Image File Name |
|---|---|
| Nike Sport Jacket | `nike_sport_jacket.jpg` |
| Adidas T-Shirt Black | `adidas_t_shirt_black.jpg` |
| Puma Essentials Pants | `puma_essentials_pants.png` |
| Supreme Box Logo | `supreme_box_logo.jpg` |
| New Balance Shoes | `new_balance_shoes.jpg` |

**Key Rules:**
- Use lowercase letters
- Replace spaces with underscores
- Use supported extensions: `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`

## Testing

Run this to verify images are loading:

```csharp
private void Form_Load(object sender, EventArgs e)
{
    // Test 1: Check available images
    var images = ImageService.GetAvailableImages();
    MessageBox.Show($"Found {images.Count} images");

    // Test 2: Try loading a product image
    var testProduct = new Product { Name = "Nike Sport Jacket" };
    string path = ImageService.BuildImagePath(testProduct.Name);
    if (!string.IsNullOrEmpty(path))
    {
        MessageBox.Show($"Image found: {path}");
    }
    else
    {
        MessageBox.Show("Image not found");
    }

    // Test 3: Load placeholder
    pictureBox1.Image = ImageService.CreatePlaceholderImage(300, 300, "Test");
}
```

## Migration from Old System

### Before (Hard-coded paths)
```csharp
string fullPath = @"D:\Projects\BrandedClothingShop\img\куртка nike.jpg";
pictureBox.Image = Image.FromFile(fullPath); // Can lock file!
```

### After (Using ImageService)
```csharp
ImageService.LoadImageToControl(pictureBox, product.ImagePath);
```

## File Organization

**Recommended structure:**
```
BrandedClothingShop/
├── Images/                    ← Put all product images here
│   ├── nike_jacket_black.jpg
│   ├── adidas_shirt_white.jpg
│   └── ...
├── Forms/
├── Services/
│   └── ImageService.cs        ← Image loading service
├── Examples/
│   └── ImageLoadingExamples.cs ← Code examples
└── ...
```

## Troubleshooting

### Images not showing?
1. ✅ Check `Images` folder exists in application directory
2. ✅ Verify file names use correct format (lowercase, underscores)
3. ✅ Ensure file extensions are supported (.jpg, .png, .bmp, .gif)
4. ✅ Run `ImageService.EnsureImagesFolderExists()` on startup

### Getting `FileNotFoundException`?
1. ✅ Use `ImageService.ImageExists(productName)` to check first
2. ✅ Verify path is relative, not absolute
3. ✅ Check file permissions in `Images` folder

### Performance issues?
1. ✅ Only load images currently visible on screen
2. ✅ Use smaller image files (optimize with image compression)
3. ✅ Avoid loading all images at once for large product lists

## Complete Minimal Example

```csharp
using BrandedClothingShop.Services;
using BrandedClothingShop.Models;
using System.Windows.Forms;

public partial class ProductForm : Form
{
    public ProductForm()
    {
        InitializeComponent();
        
        // 1. Ensure folder exists
        ImageService.EnsureImagesFolderExists();
        
        // 2. Create product
        var product = new Product
        {
            Name = "Nike Sport Jacket",
            Price = 99.99m,
            // 3. Auto-build image path
            ImagePath = ImageService.BuildImagePath("Nike Sport Jacket")
        };
        
        // 4. Load image
        ImageService.LoadImageToControl(pictureBox1, product.ImagePath);
    }
}
```

## Next Steps

1. Read `IMAGE_LOADING_GUIDE.md` for complete documentation
2. Check `ImageLoadingExamples.cs` for detailed code examples
3. Review `ImageService.cs` class for all available methods
4. Start adding product images to the `Images` folder

## Support

For issues or questions:
1. Check if folder structure is correct
2. Verify file names follow naming convention
3. Use `ImageService.GetAvailableImages()` to debug
4. Check debug output for error messages
