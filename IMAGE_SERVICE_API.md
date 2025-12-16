# ImageService API Reference

## Class: ImageService

A static utility class for loading, managing, and displaying product images in WinForms applications.

**Namespace:** `BrandedClothingShop.Services`

**File:** `Services/ImageService.cs`

---

## Public Methods

### GetImagesFolderPath()

```csharp
public static string GetImagesFolderPath()
```

**Description:** Gets the absolute path to the Images folder in the application directory.

**Returns:** 
- `string` - Absolute path to Images folder (e.g., `C:\App\Images`)

**Example:**
```csharp
string imageFolderPath = ImageService.GetImagesFolderPath();
// Returns: C:\Projects\BrandedClothingShop\Images
```

---

### BuildImagePath(productName)

```csharp
public static string BuildImagePath(string productName)
```

**Description:** Automatically builds the ImagePath property value by searching for an image file matching the product name. Returns a relative path if found, empty string if not found.

**Parameters:**
- `productName` (string) - The name of the product

**Returns:**
- `string` - Relative path like `Images/product_name.jpg`, or empty string if not found

**Behavior:**
- Normalizes product name (lowercase, replace spaces with underscores)
- Searches for matching files with supported extensions
- Falls back to partial matching if exact match not found
- Returns relative path for portability

**Example:**
```csharp
// Case 1: Exact match found
string path = ImageService.BuildImagePath("Nike Sport Jacket");
// Returns: "Images/nike_sport_jacket.jpg"

// Case 2: No match found
string path = ImageService.BuildImagePath("Unknown Product");
// Returns: ""
```

---

### LoadImage(imagePath, width, height)

```csharp
public static Image LoadImage(string imagePath, int width = 300, int height = 300)
```

**Description:** Loads an image from the specified path. Returns placeholder image if loading fails.

**Parameters:**
- `imagePath` (string) - Relative or absolute path to the image
- `width` (int, optional) - Width of placeholder image (default: 300)
- `height` (int, optional) - Height of placeholder image (default: 300)

**Returns:**
- `Image` - The loaded image or a colored placeholder image

**Exception Handling:** Returns placeholder image instead of throwing exception

**Example:**
```csharp
var image = ImageService.LoadImage("Images/nike_jacket.jpg", 400, 400);
pictureBox.Image = image;
```

---

### LoadImageToControl(pictureBox, imagePath, useDefaultColor)

```csharp
public static void LoadImageToControl(
    System.Windows.Forms.PictureBox pictureBox, 
    string imagePath, 
    bool useDefaultColor = true)
```

**Description:** Safely loads an image into a PictureBox control with automatic error handling.

**Parameters:**
- `pictureBox` (PictureBox) - The control to load the image into
- `imagePath` (string) - Relative or absolute path to the image
- `useDefaultColor` (bool, optional) - Use colored placeholder (default: true)

**Behavior:**
- Loads image without file locking
- Shows placeholder if image not found
- Handles all exceptions internally
- Works with relative paths

**Example:**
```csharp
// Basic usage
ImageService.LoadImageToControl(pictureBox1, product.ImagePath);

// With custom placeholder style
ImageService.LoadImageToControl(pictureBox1, imagePath, useDefaultColor: true);
```

---

### ImageExists(productName)

```csharp
public static bool ImageExists(string productName)
```

**Description:** Checks if an image file exists for the given product name.

**Parameters:**
- `productName` (string) - The product name to check

**Returns:**
- `bool` - True if image file exists, false otherwise

**Example:**
```csharp
if (ImageService.ImageExists("Nike Jacket"))
{
    // Load the image
    ImageService.LoadImageToControl(pictureBox, imagePath);
}
else
{
    MessageBox.Show("Image not available");
}
```

---

### GetAvailableImages()

```csharp
public static List<string> GetAvailableImages()
```

**Description:** Gets a list of all available product image file names in the Images folder.

**Returns:**
- `List<string>` - List of image file names (e.g., ["nike_jacket.jpg", "adidas_shirt.png"])

**Example:**
```csharp
var availableImages = ImageService.GetAvailableImages();
foreach (var imageName in availableImages)
{
    Console.WriteLine($"Image: {imageName}");
}
```

---

### CreatePlaceholderImage(width, height, text, useColor)

```csharp
public static Bitmap CreatePlaceholderImage(
    int width, 
    int height, 
    string text = "No Image", 
    bool useColor = true)
```

**Description:** Creates a placeholder image with text and optional colored background.

**Parameters:**
- `width` (int) - Width of the placeholder image
- `height` (int) - Height of the placeholder image
- `text` (string, optional) - Text to display on placeholder (default: "No Image")
- `useColor` (bool, optional) - Use colored background (default: true)

**Returns:**
- `Bitmap` - A new placeholder image

**Colors:** Automatically selects from 12 distinct colors based on text hash

**Example:**
```csharp
// Colored placeholder
var placeholder = ImageService.CreatePlaceholderImage(400, 400, "No Image", useColor: true);

// Gray placeholder
var placeholder = ImageService.CreatePlaceholderImage(400, 400, "Loading...", useColor: false);

pictureBox.Image = placeholder;
```

---

### EnsureImagesFolderExists()

```csharp
public static void EnsureImagesFolderExists()
```

**Description:** Creates the Images folder if it doesn't exist.

**Behavior:**
- Checks if Images folder exists in application directory
- Creates folder if missing
- Does nothing if folder already exists

**Recommended Usage:** Call in Program.Main() or Form_Load on startup

**Example:**
```csharp
public static void Main()
{
    // Initialize image system on startup
    ImageService.EnsureImagesFolderExists();
    
    Application.EnableVisualStyles();
    Application.Run(new LoginForm());
}
```

---

## Supported Image Formats

The ImageService supports these image file extensions:
- `.jpg`
- `.jpeg`
- `.png`
- `.bmp`
- `.gif`

**Case:** Extensions are case-insensitive (`.JPG` is recognized as `.jpg`)

---

## Error Handling

The ImageService handles errors gracefully:

| Scenario | Behavior |
|---|---|
| File not found | Shows placeholder image |
| Corrupted image | Shows placeholder image |
| Invalid path | Shows placeholder image |
| Access denied | Shows placeholder image |
| Out of memory | Shows placeholder image |

**Exceptions are not thrown** - use placeholder images instead for robustness.

---

## File Naming Convention

Product names are normalized using these rules:

1. Convert to lowercase
2. Trim whitespace
3. Replace invalid filename characters with underscore
4. Replace spaces with underscores
5. Collapse multiple underscores to single

**Examples:**

| Product Name | Normalized Name | File Match |
|---|---|---|
| `Nike Sport Premium Jacket` | `nike_sport_premium_jacket` | `nike_sport_premium_jacket.jpg` |
| `T-Shirt Black` | `t_shirt_black` | `t_shirt_black.png` |
| `Adidas Classic Shoes` | `adidas_classic_shoes` | `adidas_classic_shoes.jpg` |
| `Levi's 501 Jeans` | `levis_501_jeans` | `levis_501_jeans.jpg` |
| `$100 Special Deal!` | `_100_special_deal` | `_100_special_deal.jpg` |

---

## Path Handling

### Relative vs Absolute Paths

The service handles both automatically:

```csharp
// Relative path (recommended)
ImageService.LoadImageToControl(pictureBox, "Images/nike_jacket.jpg");

// Absolute path
ImageService.LoadImageToControl(pictureBox, "C:\\App\\Images\\nike_jacket.jpg");

// BuildImagePath returns relative paths
string relativePath = ImageService.BuildImagePath("Nike Jacket");
// Returns: "Images/nike_jacket.jpg"
```

### Path Safety

- Uses `Path.Combine()` for safe path construction
- Works on all Windows systems (drive letters, network paths)
- Handles special characters in paths

---

## Performance Considerations

### Memory Usage
- Images are loaded on-demand, not pre-cached
- Placeholder images are lightweight (minimal memory)
- Old images are garbage collected after use

### Disk I/O
- File existence checks are performed synchronously
- Each file is opened and closed immediately
- Suitable for typical UI operations

### UI Thread
- Image loading is synchronous (happens on UI thread)
- Safe to call from UI thread directly
- For very large images, consider async loading

---

## Placeholder Colors

When `useColor: true`, placeholders use colors from this palette:

| Color | Hex | RGB |
|---|---|---|
| Blue | #2196F3 | (33, 150, 243) |
| Green | #4CAF50 | (76, 175, 80) |
| Red | #F44336 | (244, 67, 54) |
| Pink | #E91E63 | (233, 30, 99) |
| Orange | #FF9800 | (255, 152, 0) |
| Purple | #9C27F0 | (156, 39, 176) |
| Indigo | #3F51B5 | (63, 81, 181) |
| Teal | #009688 | (0, 150, 136) |
| Amber | #FFC107 | (255, 193, 7) |
| Brown | #8B4513 | (139, 69, 19) |
| Blue Grey | #607D8B | (96, 125, 139) |
| Black | #000000 | (0, 0, 0) |

Color is selected based on hash of the placeholder text for consistency.

---

## Integration Examples

### With Product Model

```csharp
// Initialize product with auto image path
var product = new Product
{
    Name = "Nike Jacket",
    ImagePath = ImageService.BuildImagePath("Nike Jacket")
};
```

### With ProductDetailsForm

```csharp
// In form initialization
ImageService.LoadImageToControl(pictureBoxProduct, _product.ImagePath, useDefaultColor: true);
```

### With Product List

```csharp
// In product card creation
foreach (var product in products)
{
    var pictureBox = new PictureBox { Width = 200, Height = 200 };
    ImageService.LoadImageToControl(pictureBox, product.ImagePath);
    // ... add to UI
}
```

---

## Troubleshooting Guide

### Issue: Images always show placeholder

**Causes:**
1. Images folder doesn't exist
2. Image file names don't match product names
3. Image files are in wrong folder

**Solution:**
```csharp
// Check available images
var images = ImageService.GetAvailableImages();
MessageBox.Show($"Found {images.Count} images");

// Check specific product
bool exists = ImageService.ImageExists("Nike Jacket");
```

### Issue: Error messages in output

**Check:**
1. File permissions in Images folder
2. Disk space available
3. Image file corruption (try different image)

### Issue: Performance is slow

**Optimize:**
1. Load only visible images
2. Resize images to smaller dimensions
3. Use jpg instead of png for photos

---

## Version History

### Version 1.0 (Current)
- Image path building
- Safe file loading
- Placeholder generation
- Error handling
- WinForms integration

### Future Enhancements
- Image caching
- Async loading
- Thumbnail generation
- Image compression
- WebP support

---

## Related Documentation

- [Image Loading Guide](IMAGE_LOADING_GUIDE.md)
- [Quick Start Guide](QUICK_START_IMAGES.md)
- [Code Examples](Examples/ImageLoadingExamples.cs)
