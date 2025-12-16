using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public partial class CatalogForm : Form
    {
        private readonly User _user;
        private List<CartItem> _cart = new List<CartItem>();
        private FlowLayoutPanel _productsPanel = null!;
        private TextBox _searchBox = null!;
        private bool _isDarkMode = false;
        private Color _darkBgColor = Color.FromArgb(30, 30, 30);
        private Color _lightBgColor = Color.White;
        private Panel _topPanel = null!;
        private Panel _filterPanel = null!;
        private readonly string _imgFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
        
        // Фильтры
        private ComboBox _categoryFilter = null!;
        private ComboBox _brandFilter = null!;
        private ComboBox _sortFilter = null!;
        private NumericUpDown _priceMinFilter = null!;
        private NumericUpDown _priceMaxFilter = null!;

        public CatalogForm(User user)
        {
            _user = user;
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            this.Text = $"Каталог — { _user.FullName }";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = _lightBgColor;

            // Верхнє меню
            _topPanel = new Panel
            {
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var lblTitle = new Label
            {
                Text = "🏪 BRANDED CLOTHING SHOP",
                ForeColor = Color.FromArgb(255, 193, 7),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            var lblTagline = new Label
            {
                Text = "Преміальне відділення модного одягу",
                ForeColor = Color.FromArgb(180, 180, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(20, 40)
            };

            // Поле пошуку
            _searchBox = new TextBox
            {
                Width = 300,
                Height = 35,
                Location = new Point(280, 25),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "🔍 Пошук за назвою, брендом або категорією...",
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };
            _searchBox.TextChanged += (s, e) => RefreshProducts();

            var btnProfile = new Button
            {
                Text = $"👤 {_user.FullName}",
                Width = 140,
                Height = 35,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 460, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnProfile.FlatAppearance.BorderSize = 0;
            btnProfile.Click += (s, e) =>
            {
                var profileForm = new UserProfileForm(_user);
                profileForm.ShowDialog();
            };

            var btnTheme = new Button
            {
                Text = "🌙 Тема",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 305, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnTheme.FlatAppearance.BorderSize = 0;
            btnTheme.Click += (s, e) => ToggleTheme(btnTheme);

            var btnOrders = new Button
            {
                Text = "📋 Замовлення",
                Width = 130,
                Height = 35,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 195, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnOrders.FlatAppearance.BorderSize = 0;
            btnOrders.Click += (s, e) => ShowOrders();

            var btnCart = new Button
            {
                Text = $"🛒 Кошик (0)",
                Width = 140,
                Height = 35,
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 55, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCart.FlatAppearance.BorderSize = 0;
            btnCart.Click += (s, e) => ShowCart();

            // Оновлення тексту кнопки кошика
            var updateCartButton = new Action(() =>
            {
                int totalItems = _cart.Sum(ci => ci.Quantity);
                btnCart.Text = totalItems == 0 ? "🛒 Кошик" : $"🛒 Кошик ({totalItems})";
            });

            _topPanel.Controls.Add(lblTitle);
            _topPanel.Controls.Add(lblTagline);
            _topPanel.Controls.Add(_searchBox);
            _topPanel.Controls.Add(btnProfile);
            _topPanel.Controls.Add(btnTheme);
            _topPanel.Controls.Add(btnOrders);
            _topPanel.Controls.Add(btnCart);

            // Боковая панель с фильтрами
            _filterPanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(250, 250, 250),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            // Заголовок фильтров
            var lblFiltersTitle = new Label
            {
                Text = "⚙️ ФИЛЬТРЫ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            _filterPanel.Controls.Add(lblFiltersTitle);

            // Сепаратор
            var sepLine1 = new Panel
            {
                Height = 1,
                Width = 220,
                BackColor = Color.FromArgb(200, 200, 200),
                Location = new Point(15, 35)
            };
            _filterPanel.Controls.Add(sepLine1);

            // Фильтры категория
            _filterPanel.Controls.Add(new Label
            {
                Text = "КАТЕГОРИЯ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 50),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            _categoryFilter = new ComboBox
            {
                Location = new Point(15, 75),
                Width = 220,
                Height = 28,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _categoryFilter.Items.Add("Все");
            foreach (var cat in ProductService.GetAllCategories())
                _categoryFilter.Items.Add(cat);
            _categoryFilter.SelectedIndex = 0;
            _categoryFilter.SelectedIndexChanged += (s, e) => RefreshProducts();
            _filterPanel.Controls.Add(_categoryFilter);

            // Фильтр бренд
            _filterPanel.Controls.Add(new Label
            {
                Text = "БРЕНД",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 115),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            _brandFilter = new ComboBox
            {
                Location = new Point(15, 140),
                Width = 220,
                Height = 28,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _brandFilter.Items.Add("Все");
            foreach (var brand in ProductService.GetAllBrands())
                _brandFilter.Items.Add(brand);
            _brandFilter.SelectedIndex = 0;
            _brandFilter.SelectedIndexChanged += (s, e) => RefreshProducts();
            _filterPanel.Controls.Add(_brandFilter);

            // Фильтр цена
            _filterPanel.Controls.Add(new Label
            {
                Text = "ЦЕНА (₴)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 180),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            var pricePanel = new Panel
            {
                Height = 35,
                Width = 220,
                Location = new Point(15, 205)
            };

            _priceMinFilter = new NumericUpDown
            {
                Location = new Point(0, 0),
                Width = 100,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                Minimum = 0,
                Maximum = 10000,
                Value = 0
            };
            _priceMinFilter.ValueChanged += (s, e) => RefreshProducts();

            var lblPriceSep = new Label
            {
                Text = "-",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(110, 5),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            _priceMaxFilter = new NumericUpDown
            {
                Location = new Point(130, 0),
                Width = 90,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                Minimum = 0,
                Maximum = 10000,
                Value = 10000
            };
            _priceMaxFilter.ValueChanged += (s, e) => RefreshProducts();

            pricePanel.Controls.Add(_priceMinFilter);
            pricePanel.Controls.Add(lblPriceSep);
            pricePanel.Controls.Add(_priceMaxFilter);
            _filterPanel.Controls.Add(pricePanel);

            // Сортировка
            _filterPanel.Controls.Add(new Label
            {
                Text = "СОРТИРОВКА",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 250),
                ForeColor = Color.FromArgb(60, 60, 60)
            });

            _sortFilter = new ComboBox
            {
                Location = new Point(15, 275),
                Width = 220,
                Height = 28,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _sortFilter.Items.Add("🆕 Новые");
            _sortFilter.Items.Add("⭐ Популярные");
            _sortFilter.Items.Add("💰 Цена ↑");
            _sortFilter.Items.Add("💰 Цена ↓");
            _sortFilter.SelectedIndex = 0;
            _sortFilter.SelectedIndexChanged += (s, e) => RefreshProducts();
            _filterPanel.Controls.Add(_sortFilter);

            this.Controls.Add(_filterPanel);

            // Каталог товарів
            _productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            this.Controls.Add(_topPanel);
            this.Controls.Add(_productsPanel);

            // Загрузить товары
            RefreshProducts();

            // Функция обновления кнопки корзины при добавлении товара
            var originalAddToCart = new Action<Product>(p =>
            {
                var existing = _cart.FirstOrDefault(ci => ci.Product.Id == p.Id);
                if (existing != null)
                    existing.Quantity++;
                else
                    _cart.Add(new CartItem { Product = p, Quantity = 1 });
                updateCartButton();
            });

            this.Tag = originalAddToCart;
        }

        private void RefreshProducts()
        {
            _productsPanel.Controls.Clear();
            List<Product> products;

            // Получить все товары или результаты поиска
            if (string.IsNullOrWhiteSpace(_searchBox.Text))
                products = ProductService.GetAllProducts();
            else
                products = ProductService.SearchProducts(_searchBox.Text);

            // Применить фильтр по категории
            string selectedCategory = _categoryFilter?.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все")
            {
                products = products.Where(p => p.Category == selectedCategory).ToList();
            }

            // Применить фильтр по бренду
            string selectedBrand = _brandFilter?.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != "Все")
            {
                products = products.Where(p => p.Brand == selectedBrand).ToList();
            }

            // Применить фильтр по цене
            decimal minPrice = _priceMinFilter != null ? (decimal)_priceMinFilter.Value : 0;
            decimal maxPrice = _priceMaxFilter != null ? (decimal)_priceMaxFilter.Value : 10000;
            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();

            // Применить сортировку
            string sortOption = _sortFilter?.SelectedItem?.ToString();
            switch (sortOption)
            {
                case "💰 Цена ↑":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "💰 Цена ↓":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "⭐ Популярные":
                    products = products.OrderByDescending(p => p.ReviewCount).ToList();
                    break;
                default: // Новые
                    products = products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
            }

            // Отобразить товары
            foreach (var p in products)
            {
                var card = CreateProductCard(p, () =>
                {
                    var existing = _cart.FirstOrDefault(ci => ci.Product.Id == p.Id);
                    if (existing != null)
                        existing.Quantity++;
                    else
                        _cart.Add(new CartItem { Product = p, Quantity = 1 });

                    // Обновить кнопку корзины
                    var btnCart = _productsPanel.Parent?.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<Button>().FirstOrDefault(b => b.Text.Contains("Кошик"));
                    if (btnCart != null)
                    {
                        int totalItems = _cart.Sum(ci => ci.Quantity);
                        btnCart.Text = totalItems == 0 ? "🛒 Кошик" : $"🛒 Кошик ({totalItems})";
                    }
                });

                _productsPanel.Controls.Add(card);
            }

            // Если нет результатов
            if (products.Count == 0)
            {
                _productsPanel.Controls.Add(new Label
                {
                    Text = "Товари не знайдені",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 20)
                });
            }
        }

        private void ShowOrders()
        {
            var userOrders = OrderService.GetUserOrders(_user.Email);
            var ordersForm = new Form
            {
                Text = "Мої замовлення",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown
            };

            if (userOrders.Count == 0)
            {
                flow.Controls.Add(new Label
                {
                    Text = "У вас ще немає замовлень",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true
                });
            }
            else
            {
                foreach (var order in userOrders)
                {
                    var panel = new Panel
                    {
                        Width = 650,
                        Height = 120,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 10),
                        BackColor = Color.FromArgb(245, 245, 245)
                    };

                    panel.Controls.Add(new Label
                    {
                        Text = $"Замовлення №{order.Id} від {order.OrderDate:dd.MM.yyyy HH:mm}",
                        Location = new Point(10, 10),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    });

                    var itemsText = string.Join(", ", order.Items.Select(i => $"{i.Product.Name} ×{i.Quantity}"));
                    panel.Controls.Add(new Label
                    {
                        Text = $"Товари: {itemsText}",
                        Location = new Point(10, 35),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9),
                        MaximumSize = new Size(630, 40)
                    });

                    panel.Controls.Add(new Label
                    {
                        Text = $"Сума: {order.TotalPrice:C}",
                        Location = new Point(10, 70),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(255, 105, 0)
                    });

                    var statusColor = order.Status == "Доставлено" ? Color.Green : 
                                     order.Status == "Відправлено" ? Color.Orange : Color.Gray;
                    panel.Controls.Add(new Label
                    {
                        Text = $"Статус: {order.Status}",
                        Location = new Point(520, 70),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9),
                        ForeColor = statusColor
                    });

                    flow.Controls.Add(panel);
                }
            }

            ordersForm.Controls.Add(flow);
            ordersForm.ShowDialog();
        }

        private Panel CreateProductCard(Product product, Action addToCartAction)
        {
            var card = new Panel
            {
                Width = 270,
                Height = 420,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            // Тень эффект
            card.Padding = new Padding(2);

            var shadowPanel = new Panel
            {
                Width = 270,
                Height = 420,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(shadowPanel);

            var pic = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 266,
                Height = 200,
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.None
            };

            // Загрузка изображения товара по ImagePath
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                string imagePath = Path.Combine(Application.StartupPath, "Images", product.ImagePath);
                if (File.Exists(imagePath))
                {
                    pic.Image = Image.FromFile(imagePath);
                }
                else
                {
                    string placeholderPath = Path.Combine(Application.StartupPath, "Images", "placeholder.png");
                    if (File.Exists(placeholderPath))
                        pic.Image = Image.FromFile(placeholderPath);
                }
            }

            // Панель с меткой "NEW" или "СКИДКА"
            var badgePanel = new Panel { Width = 266, Height = 200, Dock = DockStyle.Top };
            if (product.IsNew)
            {
                var lblNewBadge = new Label
                {
                    Text = "🆕 NEW",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    BackColor = Color.FromArgb(244, 67, 54),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Width = 60,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(5, 5)
                };
                badgePanel.Controls.Add(lblNewBadge);
            }
            if (product.IsDiscount)
            {
                var discount = (int)((product.OriginalPrice - product.Price) / product.OriginalPrice * 100);
                var lblDiscountBadge = new Label
                {
                    Text = $"-{discount}%",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(255, 152, 0),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Width = 55,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(210, 5)
                };
                badgePanel.Controls.Add(lblDiscountBadge);
            }

            shadowPanel.Controls.Add(pic);
            shadowPanel.Controls.Add(badgePanel);

            // Информационная панель
            var infoPanel = new Panel
            {
                Width = 266,
                Height = 200,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var lblBrand = new Label
            {
                Text = product.Brand.ToUpper(),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 152, 0),
                AutoSize = true,
                Location = new Point(0, 5)
            };
            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = false,
                MaximumSize = new Size(246, 50),
                Location = new Point(0, 22),
                Height = 40
            };

            var starsText = new string('★', product.Rating) + new string('☆', 5 - product.Rating);
            var lblRating = new Label
            {
                Text = $"{starsText} ({product.ReviewCount})",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(255, 152, 0),
                AutoSize = true,
                Location = new Point(0, 68)
            };

            var lblPrice = new Label
            {
                Text = $"{product.Price:F0} ₴",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 67, 54),
                AutoSize = true,
                Location = new Point(0, 90)
            };

            if (product.IsDiscount && product.OriginalPrice > 0)
            {
                var lblOriginal = new Label
                {
                    Text = $"{product.OriginalPrice:F0} ₴",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    AutoSize = true,
                    Location = new Point(130, 100)
                };
                lblOriginal.Paint += (s, e) =>
                {
                    var textSize = e.Graphics.MeasureString(lblOriginal.Text, lblOriginal.Font);
                    e.Graphics.DrawLine(new Pen(Color.Gray, 1), 0, 8, (int)textSize.Width, 8);
                    e.Graphics.DrawString(lblOriginal.Text, lblOriginal.Font, new SolidBrush(Color.FromArgb(150, 150, 150)), 0, -3);
                };
                infoPanel.Controls.Add(lblOriginal);
            }

            var btnView = new Button
            {
                Text = "👁️ ПОДРОБНЕЕ",
                Width = 246,
                Height = 35,
                Location = new Point(0, 130),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnView.FlatAppearance.BorderSize = 0;
            btnView.Click += (s, e) =>
            {
                var detailsForm = new ProductDetailsForm(product, _user, (p, size) =>
                {
                    var existing = _cart.FirstOrDefault(ci => ci.Product.Id == p.Id && ci.Size == size);
                    if (existing != null)
                        existing.Quantity++;
                    else
                        _cart.Add(new CartItem { Product = p, Quantity = 1, Size = size });
                });
                detailsForm.ShowDialog();
            };

            var btnAdd = new Button
            {
                Text = "🛒 В КОРЗИНУ",
                Width = 246,
                Height = 30,
                Location = new Point(0, 170),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                addToCartAction();
                MessageBox.Show($"{product.Name} додано в кошик!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            btnAdd.MouseEnter += (s, e) => btnAdd.BackColor = Color.FromArgb(102, 187, 106);
            btnAdd.MouseLeave += (s, e) => btnAdd.BackColor = Color.FromArgb(76, 175, 80);

            infoPanel.Controls.Add(lblBrand);
            infoPanel.Controls.Add(lblName);
            infoPanel.Controls.Add(lblRating);
            infoPanel.Controls.Add(lblPrice);
            infoPanel.Controls.Add(btnView);
            infoPanel.Controls.Add(btnAdd);

            shadowPanel.Controls.Add(infoPanel);
            return card;
        }

        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Куртка Nike Sport", Brand = "Nike", Price = 4999.99m },
                new Product { Id = 2, Name = "Футболка Adidas Originals", Brand = "Adidas", Price = 899.50m },
                new Product { Id = 3, Name = "Штани Puma Essentials", Brand = "Puma", Price = 1499.00m },
                new Product { Id = 4, Name = "Худі Supreme Box Logo", Brand = "Supreme", Price = 8500.00m },
                new Product { Id = 5, Name = "Кросівки New Balance 574", Brand = "New Balance", Price = 3299.99m },
                new Product { Id = 6, Name = "Кепка Stüssy", Brand = "Stüssy", Price = 799.00m }
            };
        }

        private void ShowCart()
        {
            var cartForm = new Form
            {
                Text = "Ваш кошик",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var mainPanel = new Panel { Dock = DockStyle.Fill };
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown
            };

            decimal total = 0;

            if (_cart.Count == 0)
            {
                flow.Controls.Add(new Label
                {
                    Text = "Кошик порожній",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true
                });
            }
            else
            {
                foreach (var item in _cart)
                {
                    var panel = new Panel
                    {
                        Width = 520,
                        Height = 70,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 10),
                        BackColor = Color.FromArgb(250, 250, 250)
                    };

                    var itemPrice = item.Product.Price * item.Quantity;

                    panel.Controls.Add(new Label
                    {
                        Text = $"{item.Product.Name}",
                        Location = new Point(10, 10),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    });

                    panel.Controls.Add(new Label
                    {
                        Text = $"Ціна за шт: {item.Product.Price:C}",
                        Location = new Point(10, 30),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9)
                    });

                    var qtyPanel = new Panel { Location = new Point(10, 50), Width = 250, Height = 20 };
                    var btnMinus = new Button
                    {
                        Text = "-",
                        Width = 30,
                        Height = 25,
                        Location = new Point(0, 0),
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnMinus.Click += (s, e) =>
                    {
                        if (item.Quantity > 1)
                        {
                            item.Quantity--;
                            cartForm.Close();
                            ShowCart();
                        }
                    };

                    var lblQty = new Label
                    {
                        Text = item.Quantity.ToString(),
                        Location = new Point(35, 3),
                        Width = 50,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10)
                    };

                    var btnPlus = new Button
                    {
                        Text = "+",
                        Width = 30,
                        Height = 25,
                        Location = new Point(85, 0),
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnPlus.Click += (s, e) =>
                    {
                        item.Quantity++;
                        cartForm.Close();
                        ShowCart();
                    };

                    var btnRemove = new Button
                    {
                        Text = "✕ Видалити",
                        Width = 100,
                        Height = 25,
                        Location = new Point(125, 0),
                        BackColor = Color.FromArgb(255, 87, 34),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9)
                    };
                    btnRemove.Click += (s, e) =>
                    {
                        _cart.Remove(item);
                        cartForm.Close();
                        ShowCart();
                    };

                    qtyPanel.Controls.AddRange(new Control[] { btnMinus, lblQty, btnPlus, btnRemove });
                    panel.Controls.Add(qtyPanel);

                    panel.Controls.Add(new Label
                    {
                        Text = $"{itemPrice:C}",
                        Location = new Point(420, 30),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(255, 105, 0)
                    });

                    flow.Controls.Add(panel);
                    total += itemPrice;
                }
            }

            var bottomPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Bottom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var totalLabel = new Label
            {
                Text = $"Загалом: {total:C}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 0),
                Location = new Point(15, 10),
                AutoSize = true
            };

            var btnCheckout = new Button
            {
                Text = "✓ Оформити замовлення",
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(320, 15),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += (s, e) =>
            {
                if (_cart.Count > 0)
                {
                    var order = OrderService.CreateOrder(_user.Email, _cart);
                    if (order != null)
                    {
                        MessageBox.Show($"Замовлення №{order.Id} оформлено! Дякуємо за покупку ❤️\n\nСума: {order.TotalPrice:C}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _cart.Clear();
                        cartForm.Close();
                    }
                }
            };

            bottomPanel.Controls.Add(totalLabel);
            bottomPanel.Controls.Add(btnCheckout);

            mainPanel.Controls.Add(flow);
            mainPanel.Controls.Add(bottomPanel);
            cartForm.Controls.Add(mainPanel);
            cartForm.ShowDialog();
        }

        private void ToggleTheme(Button btnTheme)
        {
            _isDarkMode = !_isDarkMode;

            if (_isDarkMode)
            {
                // Переход на темную тему
                this.BackColor = _darkBgColor;
                btnTheme.Text = "☀️ Світла тема";
                btnTheme.BackColor = Color.FromArgb(100, 100, 100);

                // Обновление цветов элементов в панели товаров
                foreach (Control control in _productsPanel.Controls)
                {
                    if (control is Panel card)
                    {
                        card.BackColor = Color.FromArgb(50, 50, 50);
                        foreach (Control childControl in card.Controls)
                        {
                            if (childControl is Label label)
                            {
                                if (label.ForeColor == Color.Gray)
                                    label.ForeColor = Color.LightGray;
                                else if (label.ForeColor == Color.Black || label.ForeColor.Name == "0")
                                    label.ForeColor = Color.White;
                            }
                        }
                    }
                }
            }
            else
            {
                // Переход на светлую тему
                this.BackColor = _lightBgColor;
                btnTheme.Text = "🌙 Темна тема";
                btnTheme.BackColor = Color.FromArgb(70, 70, 70);

                // Обновление цветов элементов в панели товаров
                foreach (Control control in _productsPanel.Controls)
                {
                    if (control is Panel card)
                    {
                        card.BackColor = Color.White;
                        foreach (Control childControl in card.Controls)
                        {
                            if (childControl is Label label)
                            {
                                if (label.ForeColor == Color.LightGray)
                                    label.ForeColor = Color.Gray;
                                else if (label.ForeColor == Color.White)
                                    label.ForeColor = Color.Black;
                            }
                        }
                    }
                }
            }

            RefreshProducts();
        }
    }
}