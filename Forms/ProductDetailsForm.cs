using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class ProductDetailsForm : Form
    {
        private readonly Product _product;
        private readonly User _user;
        private Action<Product, string> _onAddToCart;
        private string _selectedSize = "";

        public ProductDetailsForm(Product product, User user, Action<Product, string> onAddToCart)
        {
            _product = product;
            _user = user;
            _onAddToCart = onAddToCart;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _product.Name;
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Верхняя панель с кнопкой закрытия
            var topPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };

            var lblBack = new LinkLabel
            {
                Text = "← Назад",
                Location = new Point(15, 12),
                AutoSize = true,
                LinkColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };
            lblBack.LinkClicked += (s, e) => this.Close();

            var lblTitle = new Label
            {
                Text = "ДЕТАЛІ ТОВАРУ",
                Location = new Point(450, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };

            topPanel.Controls.Add(lblBack);
            topPanel.Controls.Add(lblTitle);

            // Основной контент
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30)
            };

            // Контейнер для описания и изображения (горизонтальный)
            var contentContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 380,
                AutoSize = false
            };

            // Левая часть - информация
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 10, 20, 0) };

            var lblName = new Label
            {
                Text = _product.Name,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblBrand = new Label
            {
                Text = _product.Brand,
                Font = new Font("Segoe UI", 13, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(0, 35)
            };

            var lblCategory = new Label
            {
                Text = $"Категорія: {_product.Category}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(0, 60)
            };

            // Рейтинг
            var starsText = new string('★', _product.Rating) + new string('☆', 5 - _product.Rating);
            var lblRating = new Label
            {
                Text = starsText,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(229, 57, 53),
                AutoSize = true,
                Location = new Point(0, 85)
            };

            var lblPrice = new Label
            {
                Text = $"{_product.Price:C}",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 120)
            };

            var lblDescription = new Label
            {
                Text = "Опис:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 165)
            };

            var lblDescriptionText = new Label
            {
                Text = _product.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                MaximumSize = new Size(350, 100),
                Location = new Point(0, 190)
            };

            var lblSizes = new Label
            {
                Text = "Виберіть розмір:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 245)
            };

            var comboSizes = new ComboBox
            {
                Width = 200,
                Height = 30,
                Location = new Point(0, 270),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            // Заполнить ComboBox доступными размерами
            var sizes = _product.AvailableSizes.Split(',');
            foreach (var size in sizes)
            {
                comboSizes.Items.Add(size.Trim());
            }
            if (comboSizes.Items.Count > 0)
                comboSizes.SelectedIndex = 0;

            leftPanel.Controls.Add(lblName);
            leftPanel.Controls.Add(lblBrand);
            leftPanel.Controls.Add(lblCategory);
            leftPanel.Controls.Add(lblRating);
            leftPanel.Controls.Add(lblPrice);
            leftPanel.Controls.Add(lblDescription);
            leftPanel.Controls.Add(lblDescriptionText);
            leftPanel.Controls.Add(lblSizes);
            leftPanel.Controls.Add(comboSizes);

            // Правая часть - изображение
            var rightPanel = new Panel { Width = 360, Height = 380, Dock = DockStyle.Right };
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 340,
                Height = 360,
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Загрузка изображения из img по названию товара
            try
            {
                string imgDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                string[] exts = new[] { ".jpg", ".jpeg", ".png" };
                string fileNameBase = _product.Name.ToLower()
                    .Replace("'", "")
                    .Replace("'", "")
                    .Replace("`", "")
                    .Replace("-", "")
                    .Replace("ё", "е")
                    .Replace("й", "и")
                    .Replace("і", "i")
                    .Replace("ї", "i")
                    .Replace("є", "e")
                    .Replace("'", "")
                    .Replace("'", "")
                    .Replace(" ", "")
                    .Trim();
                // Маппинг для особых случаев
                var map = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"курткаnikesportpremium", "курткаnike"},
                    {"футболкаadidasclassic", "футболкаadidas"},
                    {"штанипumaessentials", "штанипuma"},
                    {"худisupremeboxlogo", "худsupreme"},
                    {"кросівкиnewbalance574", "кросівкиnewbalance"},
                    {"кепкаstüssyclassic", "кепкаstussy"},
                    {"толстовкаcarhartrugged", "толстовкаcarhartt"},
                    {"джинсilevi501original", "джинсilevis"},
                    {"рубашкахugoboss", "рубашкаhugoboss"},
                    {"спортивнічеревикиtimberland", "спортивнічеревикиtimberland"},
                    {"світшотcalvinklein", "світшотcalvinklein"},
                    {"очкиrry-banaviator", "очкиrraybanaviator"}
                };
                string fileName = fileNameBase;
                if (map.ContainsKey(fileNameBase))
                    fileName = map[fileNameBase];
                string? foundPath = null;
                foreach (var ext in exts)
                {
                    var path = System.IO.Path.Combine(imgDir, fileName + ext);
                    if (System.IO.File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }
                if (foundPath != null)
                {
                    pictureBox.Image = Image.FromFile(foundPath);
                }
                else
                {
                    // fallback: цветная заглушка
                    var bitmap = new Bitmap(340, 360);
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        Color[] colors = new[]
                        {
                            Color.FromArgb(33, 150, 243),
                            Color.FromArgb(76, 175, 80),
                            Color.FromArgb(244, 67, 54),
                            Color.FromArgb(233, 30, 99),
                            Color.FromArgb(255, 152, 0),
                            Color.FromArgb(156, 39, 176),
                            Color.FromArgb(63, 81, 181),
                            Color.FromArgb(0, 150, 136),
                            Color.FromArgb(255, 193, 7),
                            Color.FromArgb(139, 69, 19),
                            Color.FromArgb(96, 125, 139),
                            Color.FromArgb(0, 0, 0)
                        };
                        var bgColor = colors[Math.Min(_product.Id - 1, colors.Length - 1)];
                        graphics.Clear(bgColor);
                        var font = new Font("Segoe UI", 18, FontStyle.Bold);
                        var brush = new SolidBrush(Color.White);
                        var stringFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        graphics.DrawString(_product.Name, font, brush,
                            new Rectangle(20, 140, 300, 80), stringFormat);
                    }
                    pictureBox.Image = bitmap;
                }
            }
            catch
            {
                pictureBox.BackColor = Color.FromArgb(230, 230, 230);
            }

            rightPanel.Controls.Add(pictureBox);

            contentContainer.Controls.Add(leftPanel);
            contentContainer.Controls.Add(rightPanel);
            mainPanel.Controls.Add(contentContainer);

            // Секция характеристик
            var charPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10)
            };

            var lblSpecs = new Label
            {
                Text = "Характеристики:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var specs = new Label
            {
                Text = $"ID: {_product.Id}\nБренд: {_product.Brand}\nКатегорія: {_product.Category}\nОцінка: {_product.Rating}/5",
                Font = new Font("Segoe UI", 10),
                Location = new Point(0, 25),
                AutoSize = true
            };

            charPanel.Controls.Add(lblSpecs);
            charPanel.Controls.Add(specs);
            mainPanel.Controls.Add(charPanel);

            var btnPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var btnAddReview = new Button
            {
                Text = "ДОДАТИ Відгук",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 60)
            };
            btnAddReview.FlatAppearance.BorderSize = 0;
            btnAddReview.Click += (s, e) =>
            {
                var reviewForm = new ReviewForm(_product, _user);
                if (reviewForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Дякуємо за ваш відгук!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var btnAddToCart = new Button
            {
                Text = "ДОДАТИ В КОШИК",
                Width = 250,
                Height = 40,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(720, 10)
            };
            btnAddToCart.FlatAppearance.BorderSize = 0;
            btnAddToCart.Click += (s, e) =>
            {
                if (comboSizes.SelectedIndex < 0)
                {
                    MessageBox.Show("Будь ласка, виберіть розмір!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                _selectedSize = comboSizes.SelectedItem?.ToString() ?? "";
                _onAddToCart(_product, _selectedSize);
                MessageBox.Show($"{_product.Name} (Розмір: {_selectedSize}) додано в кошик!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };

            btnPanel.Controls.Add(btnAddToCart);
            btnPanel.Controls.Add(btnAddReview);

            this.Controls.Add(mainPanel);
            this.Controls.Add(btnPanel);
            this.Controls.Add(topPanel);
        }
    }
}
