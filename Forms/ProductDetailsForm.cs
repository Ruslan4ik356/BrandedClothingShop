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
        private int _quantity = 1;

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
            this.Size = new Size(1400, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Верхняя панель с кнопкой закрытия
            var topPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };

            var lblBack = new LinkLabel
            {
                Text = "← Назад до каталога",
                Location = new Point(20, 18),
                AutoSize = true,
                LinkColor = Color.FromArgb(255, 193, 7),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            lblBack.LinkClicked += (s, e) => this.Close();

            var lblTitle = new Label
            {
                Text = "🛍️ ДЕТАЛЬНИЙ ПЕРЕГЛЯД ТОВАРУ",
                Location = new Point(500, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7)
            };

            topPanel.Controls.Add(lblBack);
            topPanel.Controls.Add(lblTitle);

            // Основной контент
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(40)
            };

            // Главный контейнер (левая часть - изображение, правая часть - информация)
            var contentContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 500,
                AutoSize = false
            };

            // ЛІВА ЧАСТИНА - Зображення товару
            var imgPanel = new Panel 
            { 
                Width = 450, 
                Height = 500, 
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 420,
                Height = 470,
                Location = new Point(15, 15),
                BorderStyle = BorderStyle.None
            };

            // Завантаження зображення товару за ImagePath
            if (!string.IsNullOrEmpty(_product.ImagePath))
            {
                string imagePath = Path.Combine(Application.StartupPath, "Images", _product.ImagePath);
                if (File.Exists(imagePath))
                {
                    pictureBox.Image = Image.FromFile(imagePath);
                }
                else
                {
                    string placeholderPath = Path.Combine(Application.StartupPath, "Images", "placeholder.png");
                    if (File.Exists(placeholderPath))
                        pictureBox.Image = Image.FromFile(placeholderPath);
                }
            }

            imgPanel.Controls.Add(pictureBox);

            // ПРАВАЯ ЧАСТЬ - Информация о товаре
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(30, 10, 30, 0)
            };

            // Название и бренд
            var lblBrand = new Label
            {
                Text = _product.Brand.ToUpper(),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblName = new Label
            {
                Text = _product.Name,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = false,
                MaximumSize = new Size(700, 80),
                Location = new Point(0, 25),
                ForeColor = Color.Black
            };

            // Категория и рейтинг в одной строке
            var lblCategory = new Label
            {
                Text = $"Категорія: {_product.Category}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(0, 105)
            };

            var starsText = new string('★', _product.Rating) + new string('☆', 5 - _product.Rating);
            var lblRating = new Label
            {
                Text = $"{starsText} ({_product.ReviewCount} відгуків)",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(255, 152, 0),
                AutoSize = true,
                Location = new Point(0, 130)
            };

            // Цена
            var pricePanel = new Panel
            {
                Height = 50,
                Location = new Point(0, 160),
                AutoSize = false
            };

            var lblPrice = new Label
            {
                Text = $"{_product.Price:F2} ₴",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 67, 54),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            if (_product.IsDiscount && _product.OriginalPrice > 0)
            {
                var lblOriginalPrice = new Label
                {
                    Text = $"{_product.OriginalPrice:F2} ₴",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    AutoSize = true,
                    Location = new Point(280, 10)
                };
                lblOriginalPrice.Paint += (s, e) =>
                {
                    var textSize = e.Graphics.MeasureString(lblOriginalPrice.Text, lblOriginalPrice.Font);
                    e.Graphics.DrawLine(Pens.Gray, 0, 8, (int)textSize.Width, 8);
                    e.Graphics.DrawString(lblOriginalPrice.Text, lblOriginalPrice.Font, Brushes.Gray, 0, -3);
                };
                pricePanel.Controls.Add(lblOriginalPrice);
            }

            pricePanel.Controls.Add(lblPrice);

            // Опис товару
            var lblDescTitle = new Label
            {
                Text = "ОПИС ТОВАРУ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 230)
            };

            var lblDescription = new Label
            {
                Text = _product.Description,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = false,
                MaximumSize = new Size(700, 200),
                Location = new Point(0, 260),
                Height = 100
            };

            // Доступные цвета
            var lblColorsTitle = new Label
            {
                Text = "ДОСТУПНЫЕ ЦВЕТА:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 360)
            };

            var colorsPanel = new Panel
            {
                Height = 40,
                AutoSize = false,
                Location = new Point(0, 385)
            };

            if (_product.Colors != null && _product.Colors.Count > 0)
            {
                int colorX = 0;
                foreach (var color in _product.Colors)
                {
                    var colorBtn = new Button
                    {
                        Width = 35,
                        Height = 35,
                        Location = new Point(colorX, 0),
                        BackColor = GetColorFromName(color),
                        FlatStyle = FlatStyle.Flat
                    };
                    colorBtn.FlatAppearance.BorderSize = 2;
                    colorBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                    colorsPanel.Controls.Add(colorBtn);
                    colorX += 45;
                }
            }

            // Размеры
            var lblSizesTitle = new Label
            {
                Text = "ВЫБЕРИТЕ РАЗМЕР:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 435)
            };

            var comboSizes = new ComboBox
            {
                Width = 300,
                Height = 30,
                Location = new Point(0, 460),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };

            var sizes = _product.AvailableSizes.Split(',');
            foreach (var size in sizes)
            {
                comboSizes.Items.Add(size.Trim());
            }
            if (comboSizes.Items.Count > 0)
                comboSizes.SelectedIndex = 0;

            infoPanel.Controls.Add(lblBrand);
            infoPanel.Controls.Add(lblName);
            infoPanel.Controls.Add(lblCategory);
            infoPanel.Controls.Add(lblRating);
            infoPanel.Controls.Add(pricePanel);
            infoPanel.Controls.Add(lblDescTitle);
            infoPanel.Controls.Add(lblDescription);
            infoPanel.Controls.Add(lblColorsTitle);
            infoPanel.Controls.Add(colorsPanel);
            infoPanel.Controls.Add(lblSizesTitle);
            infoPanel.Controls.Add(comboSizes);

            contentContainer.Controls.Add(infoPanel);
            contentContainer.Controls.Add(imgPanel);

            mainPanel.Controls.Add(contentContainer);

            // Нижняя панель с кнопками действий
            var btnPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(40, 20, 40, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кількість товару
            var lblQuantity = new Label
            {
                Text = "Количество:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 15)
            };

            var numQuantity = new NumericUpDown
            {
                Value = 1,
                Minimum = 1,
                Maximum = 999,
                Width = 80,
                Height = 35,
                Location = new Point(120, 10),
                Font = new Font("Segoe UI", 10)
            };
            numQuantity.ValueChanged += (s, e) => _quantity = (int)numQuantity.Value;

            // Кнопка добавления в корзину
            var btnAddToCart = new Button
            {
                Text = "🛒 ДОБАВИТЬ В КОРЗИНУ",
                Width = 280,
                Height = 50,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(650, 15)
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
                for (int i = 0; i < _quantity; i++)
                {
                    _onAddToCart(_product, _selectedSize);
                }
                MessageBox.Show($"{_product.Name} (Розмір: {_selectedSize}) x{_quantity} додано в кошик!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };
            btnAddToCart.MouseEnter += (s, e) => btnAddToCart.BackColor = Color.FromArgb(229, 57, 53);
            btnAddToCart.MouseLeave += (s, e) => btnAddToCart.BackColor = Color.FromArgb(244, 67, 54);

            // Кнопка додавання відзиву
            var btnAddReview = new Button
            {
                Text = "✍️ ДОДАТИ ВІДЗИВ",
                Width = 280,
                Height = 50,
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(950, 15)
            };
            btnAddReview.FlatAppearance.BorderSize = 0;
            btnAddReview.Click += (s, e) =>
            {
                var reviewForm = new ReviewForm(_product, _user);
                if (reviewForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Дякуємо за ваш відзив!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            btnAddReview.MouseEnter += (s, e) => btnAddReview.BackColor = Color.FromArgb(253, 204, 5);
            btnAddReview.MouseLeave += (s, e) => btnAddReview.BackColor = Color.FromArgb(255, 193, 7);

            btnPanel.Controls.Add(lblQuantity);
            btnPanel.Controls.Add(numQuantity);
            btnPanel.Controls.Add(btnAddToCart);
            btnPanel.Controls.Add(btnAddReview);

            this.Controls.Add(mainPanel);
            this.Controls.Add(btnPanel);
            this.Controls.Add(topPanel);
        }

        private Color GetColorFromName(string colorName)
        {
            return colorName.ToLower() switch
            {
                "black" => Color.FromArgb(0, 0, 0),
                "white" => Color.FromArgb(255, 255, 255),
                "red" => Color.FromArgb(255, 0, 0),
                "blue" => Color.FromArgb(0, 0, 255),
                "green" => Color.FromArgb(0, 128, 0),
                "navy" => Color.FromArgb(0, 0, 128),
                "beige" => Color.FromArgb(245, 245, 220),
                "brown" => Color.FromArgb(165, 42, 42),
                _ => Color.FromArgb(200, 200, 200)
            };
        }
    }
}
