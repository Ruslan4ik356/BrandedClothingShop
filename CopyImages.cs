using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string source = @"d:\Документы\GitHub\BrandedClothingShop\img";
        string dest = @"d:\Документы\GitHub\BrandedClothingShop\Images";

        if (!Directory.Exists(source))
        {
            Console.WriteLine($"Папка не найдена: {source}");
            return;
        }

        if (!Directory.Exists(dest))
        {
            Directory.CreateDirectory(dest);
        }

        var files = Directory.GetFiles(source);
        Console.WriteLine($"Найдено файлов: {files.Length}");

        foreach (var file in files)
        {
            try
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(dest, fileName);
                File.Copy(file, destFile, true);
                Console.WriteLine($"✓ {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка: {ex.Message}");
            }
        }

        int count = Directory.GetFiles(dest).Length;
        Console.WriteLine($"\nВсего скопировано: {count} файлов");
    }
}
