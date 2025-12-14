using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class CatalogFormModern : Form
    {
        private readonly User _user;
        private List<CartItem> _cart = new List<CartItem>();
        private CartForm _currentCartForm = null!;
        private Panel _productsPanel = null!;
        private TextBox _searchBox = null!;
        private Button _cartButton = null!;
        private Label _cartCountLabel = null!;
        private ComboBox _sortCombo = null!;
        private ComboBox _categoryCombo = null!;

        public CatalogFormModern(User user)
        {
            _user = user;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "BRANDED — Магазин Моди";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // ========== ВЕРХНЯЯ НАВИГАЦИОННАЯ ПАНЕЛЬ ==========
            var topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };

            // Логотип
            var lblLogo = new Label
            {
                Text = "BRANDED",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 15),
                AutoSize = true
            };

            // Поле поиска
            _searchBox = new TextBox
            {
                Width = 350,
                Height = 35,
                Location = new Point(450, 17),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Пошук товарів...",
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(51, 51, 51),
                BorderStyle = BorderStyle.FixedSingle
            };
            _searchBox.TextChanged += (s, e) => RefreshProducts();

            // Кнопка корзины (справа)
            _cartButton = new Button
            {
                Text = "🛒",
                Width = 50,
                Height = 35,
                Location = new Point(1310, 17),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16),
                FlatStyle = FlatStyle.Flat
            };
            _cartButton.Click += (s, e) => ShowCart();

            // Счетчик товаров в корзине
            _cartCountLabel = new Label
            {
                Text = "0",
                Width = 30,
                Height = 30,
                Location = new Point(1360, 20),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Visible = false,
                BorderStyle = BorderStyle.None
            };

            // Кнопка профиля
            var btnProfile = new Button
            {
                Text = "👤 " + _user.FullName.Split(' ')[0],
                Width = 150,
                Height = 35,
                Location = new Point(1150, 17),
                BackColor = Color.FromArgb(51, 51, 51),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            btnProfile.Click += (s, e) =>
            {
                var profileForm = new UserProfileForm(_user);
                profileForm.ShowDialog();
            };

            // Кнопка истории заказов
            var btnOrders = new Button
            {
                Text = "📋",
                Width = 50,
                Height = 35,
                Location = new Point(1300, 17),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14),
                FlatStyle = FlatStyle.Flat
            };
            btnOrders.Click += (s, e) => ShowOrders();
            btnOrders.FlatAppearance.BorderSize = 0;

            // Кнопка переключения темы
            var btnTheme = new Button
            {
                Text = "🌙",
                Width = 50,
                Height = 35,
                Location = new Point(1300, 17),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14),
                FlatStyle = FlatStyle.Flat
            };
            btnTheme.Click += (s, e) =>
            {
                ThemeManager.CurrentTheme = ThemeManager.CurrentTheme == ThemeManager.Theme.Light 
                    ? ThemeManager.Theme.Dark 
                    : ThemeManager.Theme.Light;
                MessageBox.Show($"Тема змінена на {ThemeManager.CurrentTheme}!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Перезагружаем форму для применения темы
                this.BackColor = ThemeManager.GetBackgroundColor();
            };
            btnTheme.FlatAppearance.BorderSize = 0;

            // Кнопка выхода
            var btnLogout = new Button
            {
                Text = "🚪",
                Width = 50,
                Height = 35,
                Location = new Point(1400, 17),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14),
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.Click += (s, e) =>
            {
                new LoginForm().Show();
                this.Close();
            };

            topPanel.Controls.Add(lblLogo);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_cartButton);
            topPanel.Controls.Add(_cartCountLabel);
            topPanel.Controls.Add(btnProfile);
            topPanel.Controls.Add(btnOrders);
            topPanel.Controls.Add(btnTheme);
            topPanel.Controls.Add(btnLogout);

            // ========== БОКОВАЯ ПАНЕЛЬ ФИЛЬТРОВ ==========
            var sidePanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            // Сортировка
            var lblSort = new Label
            {
                Text = "СОРТУВАННЯ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 15)
            };

            _sortCombo = new ComboBox
            {
                Location = new Point(15, 40),
                Width = 220,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "За назвою", "Ціна: низька", "Ціна: висока", "Топ рейтинг" },
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            _sortCombo.SelectedIndex = 0;
            _sortCombo.SelectedIndexChanged += (s, e) => RefreshProducts();

            // Категория
            var lblCategory = new Label
            {
                Text = "КАТЕГОРІЯ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 80)
            };

            _categoryCombo = new ComboBox
            {
                Location = new Point(15, 105),
                Width = 220,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Всі", "Куртки", "Сорочки", "Штани", "Взуття", "Аксесуари" },
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            _categoryCombo.SelectedIndex = 0;
            _categoryCombo.SelectedIndexChanged += (s, e) => RefreshProducts();

            // Фільтр за ціною
            var lblPrice = new Label
            {
                Text = "ЦІНА",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 150)
            };

            var lblMinPrice = new Label { Text = "Від:", Location = new Point(15, 175), AutoSize = true };
            var txtMinPrice = new TextBox { Location = new Point(60, 172), Width = 60, Text = "0", BorderStyle = BorderStyle.FixedSingle };

            var lblMaxPrice = new Label { Text = "До:", Location = new Point(135, 175), AutoSize = true };
            var txtMaxPrice = new TextBox { Location = new Point(165, 172), Width = 70, Text = "9999", BorderStyle = BorderStyle.FixedSingle };

            var btnPriceFilter = new Button
            {
                Text = "Застосувати",
                Width = 220,
                Height = 28,
                Location = new Point(15, 200),
                BackColor = Color.FromArgb(51, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnPriceFilter.Click += (s, e) =>
            {
                if (decimal.TryParse(txtMinPrice.Text, out decimal minPrice) && decimal.TryParse(txtMaxPrice.Text, out decimal maxPrice))
                {
                    RefreshProducts(minPrice, maxPrice);
                }
            };

            sidePanel.Controls.Add(lblSort);
            sidePanel.Controls.Add(_sortCombo);
            sidePanel.Controls.Add(lblCategory);
            sidePanel.Controls.Add(_categoryCombo);
            sidePanel.Controls.Add(lblPrice);
            sidePanel.Controls.Add(lblMinPrice);
            sidePanel.Controls.Add(txtMinPrice);
            sidePanel.Controls.Add(lblMaxPrice);
            sidePanel.Controls.Add(txtMaxPrice);
            sidePanel.Controls.Add(btnPriceFilter);

            // ========== ОСНОВНАЯ ПАНЕЛЬ ТОВАРОВ ==========
            _productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20),
                Margin = new Padding(0)
            };

            this.Controls.Add(_productsPanel);
            this.Controls.Add(sidePanel);
            this.Controls.Add(topPanel);

            RefreshProducts();
        }

        private void RefreshProducts()
        {
            RefreshProducts(0, 9999);
        }

        private void RefreshProducts(decimal minPrice, decimal maxPrice)
        {
            _productsPanel.Controls.Clear();
            var products = ProductService.GetAllProducts();

            // Поиск
            if (!string.IsNullOrWhiteSpace(_searchBox.Text))
                products = products.Where(p => p.Name.ToLower().Contains(_searchBox.Text.ToLower()) ||
                                              p.Brand.ToLower().Contains(_searchBox.Text.ToLower())).ToList();

            // Фильтр по цене
            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();

            // Фильтр по категории
            if (_categoryCombo.SelectedIndex > 0)
            {
                var selectedCategory = _categoryCombo.SelectedItem?.ToString();
                if (selectedCategory != null)
                    products = products.Where(p => p.Category == selectedCategory).ToList();
            }

            // Сортировка
            var sortIndex = _sortCombo.SelectedIndex;
            products = sortIndex switch
            {
                1 => products.OrderBy(p => p.Price).ToList(),
                2 => products.OrderByDescending(p => p.Price).ToList(),
                3 => products.OrderByDescending(p => p.Rating).ToList(),
                _ => products.OrderBy(p => p.Name).ToList()
            };

            if (products.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "😞 Товари не знайдені",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                _productsPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var product in products)
            {
                var card = CreateProductCard(product);
                _productsPanel.Controls.Add(card);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Width = 240,
                Height = 360,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10)
            };

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(250, 250, 250);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            // Изображение
            var pic = new PictureBox
            {
                Width = 240,
                Height = 240,
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.None,
                Cursor = Cursors.Hand
            };

            try
            {
                // Пытаемся загрузить локальное изображение
                string localImagePath = $"img/{product.Id}.jpg";
                if (System.IO.File.Exists(localImagePath))
                {
                    pic.Image = Image.FromFile(localImagePath);
                }
                else
                {
                    // Если локального файла нет, генерируем цветное изображение
                    GenerateColoredProductImage(pic, product);
                }
            }
            catch
            {
                // Если локальный файл не открывается, генерируем цветное изображение
                GenerateColoredProductImage(pic, product);
            }

            pic.Click += (s, e) => ShowProductDetails(product);

            // Название
            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(0, 250),
                Width = 240,
                Height = 25,
                AutoSize = false,
                AutoEllipsis = true,
                ForeColor = Color.Black
            };

            // Бренд
            var lblBrand = new Label
            {
                Text = product.Brand,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 278),
                AutoSize = true
            };

            // Цена
            var lblPrice = new Label
            {
                Text = $"{product.Price:C}",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(0, 300),
                AutoSize = true
            };

            // Кнопка "Деталі"
            var btnDetails = new Button
            {
                Text = "Деталі",
                Width = 115,
                Height = 32,
                Location = new Point(0, 325),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnDetails.FlatAppearance.BorderSize = 0;
            btnDetails.Click += (s, e) => ShowProductDetails(product);

            // Кнопка "Додати в кошик"
            var btnCart = new Button
            {
                Text = "Додати",
                Width = 115,
                Height = 32,
                Location = new Point(125, 325),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCart.FlatAppearance.BorderSize = 0;
            btnCart.Click += (s, e) => AddToCart(product);

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblBrand);
            card.Controls.Add(lblPrice);
            card.Controls.Add(btnDetails);
            card.Controls.Add(btnCart);

            return card;
        }

        private void GenerateColoredProductImage(PictureBox pic, Product product)
        {
            try
            {
                var bitmap = new Bitmap(240, 240);
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

                    var bgColor = colors[Math.Min(product.Id - 1, colors.Length - 1)];
                    graphics.Clear(bgColor);
                    
                    var font = new Font("Segoe UI", 16, FontStyle.Bold);
                    var brush = new SolidBrush(Color.White);
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    
                    graphics.DrawString(product.Name.Substring(0, Math.Min(10, product.Name.Length)), 
                        font, brush, new Rectangle(10, 90, 220, 60), stringFormat);
                }

                pic.Image = bitmap;
            }
            catch
            {
                pic.BackColor = Color.FromArgb(230, 230, 230);
            }
        }

        private void ShowProductDetails(Product product)
        {
            var detailsForm = new ProductDetailsForm(product, _user, (p, size) => AddToCart(p, size));
            detailsForm.ShowDialog();
        }

        private void AddToCart(Product product, string size = "M")
        {
            var existing = _cart.FirstOrDefault(ci => ci.Product.Id == product.Id && ci.Size == size);
            if (existing != null)
                existing.Quantity++;
            else
                _cart.Add(new CartItem { Product = product, Quantity = 1, Size = size });

            UpdateCartButton();

            if (_currentCartForm != null && !_currentCartForm.IsDisposed)
                _currentCartForm.RefreshUI(_cart);

            MessageBox.Show($"✅ {product.Name} (Size: {size}) додано до кошика!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCartButton()
        {
            int totalItems = _cart.Sum(ci => ci.Quantity);
            if (totalItems > 0)
            {
                _cartCountLabel.Visible = true;
                _cartCountLabel.Text = totalItems > 99 ? "99+" : totalItems.ToString();
            }
            else
            {
                _cartCountLabel.Visible = false;
            }
        }

        private void ShowCart()
        {
            _currentCartForm = new CartForm(_user, _cart, (updatedCart) =>
            {
                _cart = updatedCart;
                UpdateCartButton();
            });

            _currentCartForm.FormClosed += (s, e) => _currentCartForm = null!;
            _currentCartForm.Show();
        }

        private void ShowOrders()
        {
            var orderHistoryForm = new OrderHistoryForm(_user);
            orderHistoryForm.ShowDialog();
        }
    }
}
