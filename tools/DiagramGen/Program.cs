using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

class Program
{
    static void Main()
    {
        int width = 2400;
        int height = 1500;
        using var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.White);

        var titleFont = new Font("Segoe UI", 24, FontStyle.Bold);
        var captionFont = new Font("Segoe UI", 12, FontStyle.Regular);
        var ucFont = new Font("Segoe UI", 14, FontStyle.Regular);
        var boldFont = new Font("Segoe UI", 14, FontStyle.Bold);
        var pen = new Pen(Color.Black, 2);
        var dashPen = new Pen(Color.Gray, 2) { DashStyle = DashStyle.Dash };

        // System frame
        var frameRect = new Rectangle(220, 80, width - 260, height - 160);
        g.DrawRectangle(pen, frameRect);
        g.DrawString("BrandedClothingShop", boldFont, Brushes.Black, frameRect.Left + 10, frameRect.Top - 30);

        // Actor (User) at left
        var actorX = 100; var actorY = height / 2 - 100;
        DrawActor(g, actorX, actorY);
        g.DrawString("Користувач", ucFont, Brushes.Black, actorX - 30, actorY + 180);

        // Helper to draw a use-case ellipse with text, returns center point
        Point DrawUC(string text, int cx, int cy)
        {
            var size = g.MeasureString(text, ucFont);
            int rx = (int)Math.Max(100, size.Width / 2 + 30);
            int ry = 40;
            var rect = new Rectangle(cx - rx, cy - ry, rx * 2, ry * 2);
            g.FillEllipse(Brushes.WhiteSmoke, rect);
            g.DrawEllipse(pen, rect);
            StringFormat sf = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, ucFont, Brushes.Black, rect, sf);
            return new Point(cx, cy);
        }

        void Link(Point from, Point to, bool dashed = false, string? label = null)
        {
            var p = dashed ? dashPen : pen;
            g.DrawLine(p, from, to);
            if (!string.IsNullOrEmpty(label))
            {
                var mid = new Point((from.X + to.X) / 2, (from.Y + to.Y) / 2);
                g.DrawString(label!, captionFont, Brushes.Gray, mid);
            }
        }

        // Public use cases
        var uc1 = DrawUC("UC1 Реєстрація", frameRect.Left + 420, frameRect.Top + 160);
        var uc2 = DrawUC("UC2 Авторизація", frameRect.Left + 420, frameRect.Top + 260);
        var uc3 = DrawUC("UC3 Переглянути каталог", frameRect.Left + 900, frameRect.Top + 280);
        var uc4 = DrawUC("UC4 Пошук", frameRect.Left + 680, frameRect.Top + 380);
        var uc5 = DrawUC("UC5 Фільтри (бренд/катег./ціна)", frameRect.Left + 900, frameRect.Top + 380);
        var uc6 = DrawUC("UC6 Сортування (популярн./ціна/новизна)", frameRect.Left + 1140, frameRect.Top + 380);
        var uc7 = DrawUC("UC7 Деталі товару", frameRect.Left + 1230, frameRect.Top + 220);

        // Authorized use cases block (right side)
        var uc8  = DrawUC("UC8 Додати у кошик (з розміром)", frameRect.Left + 680, frameRect.Top + 600);
        var uc9  = DrawUC("UC9 Керувати кошиком", frameRect.Left + 930, frameRect.Top + 600);
        var uc10 = DrawUC("UC10 Оформити замовлення", frameRect.Left + 1210, frameRect.Top + 600);
        var uc11 = DrawUC("UC11 Обрати спосіб доставки", frameRect.Left + 1100, frameRect.Top + 700);
        var uc12 = DrawUC("UC12 Розрахувати вартість доставки", frameRect.Left + 1330, frameRect.Top + 700);
        var uc13 = DrawUC("UC13 Переглянути підсумок замовлення", frameRect.Left + 1220, frameRect.Top + 800);
        var uc14 = DrawUC("UC14 Історія замовлень", frameRect.Left + 1480, frameRect.Top + 560);
        var uc15 = DrawUC("UC15 Деталі замовлення", frameRect.Left + 1680, frameRect.Top + 640);
        var uc16 = DrawUC("UC16 Додати рецензію", frameRect.Left + 1400, frameRect.Top + 360);
        var uc17 = DrawUC("UC17 Переглянути рецензії", frameRect.Left + 1520, frameRect.Top + 260);
        var uc18 = DrawUC("UC18 Керувати профілем", frameRect.Left + 820, frameRect.Top + 770);
        var uc19 = DrawUC("UC19 Wishlist", frameRect.Left + 980, frameRect.Top + 770);
        var uc20 = DrawUC("UC20 Вийти", frameRect.Left + 420, frameRect.Top + 360);

        // Actor links (basic associations)
        var actorCenter = new Point(actorX + 30, actorY + 70);
        foreach (var pnt in new[] { uc1, uc2, uc3, uc7, uc8, uc9, uc10, uc14, uc16, uc17, uc18, uc19, uc20 })
            Link(actorCenter, pnt);

        // Includes/Extends
        Link(uc3, uc4, dashed: true, label: "<<include>>");
        Link(uc3, uc5, dashed: true, label: "<<include>>");
        Link(uc3, uc6, dashed: true, label: "<<include>>");

        Link(uc10, uc11, dashed: true, label: "<<include>>");
        Link(uc10, uc12, dashed: true, label: "<<include>>");
        Link(uc10, uc13, dashed: true, label: "<<include>>");

        Link(uc14, uc15, dashed: true, label: "<<include>>");

        Link(uc16, uc7, dashed: true, label: "<<extend>>");

        Link(uc10, uc2, dashed: true, label: "<<extend>> (лише після входу)");

        // Legend note
        g.DrawString("Примітка: дії UC8–UC19 доступні після авторизації (UC2)", captionFont, Brushes.DimGray, frameRect.Left + 560, frameRect.Bottom - 30);

        // Output
        var outDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "img", "diagrams");
        Directory.CreateDirectory(outDir);
        var outPath = Path.Combine(outDir, "usecase_user.png");
        bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
        Console.WriteLine(outPath);
    }

    static void DrawActor(Graphics g, int x, int y)
    {
        using var pen = new Pen(Color.Black, 3);
        // Head
        g.DrawEllipse(pen, x + 10, y, 40, 40);
        // Body
        g.DrawLine(pen, x + 30, y + 40, x + 30, y + 110);
        // Arms
        g.DrawLine(pen, x - 10, y + 70, x + 70, y + 70);
        // Legs
        g.DrawLine(pen, x + 30, y + 110, x, y + 170);
        g.DrawLine(pen, x + 30, y + 110, x + 60, y + 170);
    }
}
