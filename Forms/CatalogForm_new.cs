using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public partial class CatalogFormNew : Form
    {
        private readonly User _user;
        private List<CartItem> _cart = new List<CartItem>();
        private FlowLayoutPanel _productsPanel = null!;
        private TextBox _searchBox = null!;
        private Button _btnCart = null!;
        private Label _cartCountLabel = null!;
        private CartForm _currentCartForm = null!;

        public CatalogFormNew(User user)
        {
            _user = user;
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            this.Text = $"BrandedClothingShop — {_user.FullName}";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 245, 250);
            this.Font = new Font("Segoe UI", 10);

            // Верхняя панель - навигация и поиск (увеличенная высота)
            var topPanel = new Panel
            {
                Height = 140,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(120, 100, 60),
                Padding = new Padding(20)
            };

            // Логотип / название
            var lblTitle = new Label
            {
                Text = "👕 БРЕНДОВАНА МОДА",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 5)
            };

            // Поле поиска
            _searchBox = new TextBox
            {
                Width = 350,
                Height = 40,
                Location = new Point(450, 50),
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "🔍 Пошук товару...",
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            _searchBox.TextChanged += (s, e) => RefreshProducts();

            // Кнопка корзины (справа вверху)
            _btnCart = new Button
            {
                Text = "🛒 КОШИК",
                Width = 120,
                Height = 40,
                Location = new Point(this.ClientSize.Width - 520, 50),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            _btnCart.FlatAppearance.BorderSize = 0;
            _btnCart.Click += (s, e) => ShowCart();

            // Счетчик корзины
            _cartCountLabel = new Label
            {
                Text = "0",
                Width = 28,
                Height = 28,
                Location = new Point(this.ClientSize.Width - 415, 50),
                BackColor = Color.FromArgb(180, 140, 100),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Visible = false,
                BorderStyle = BorderStyle.None
            };

            // Кнопка заказов
            var btnOrders = new Button
            {
                Text = "📋 ИСТОРИЯ",
                Width = 120,
                Height = 40,
                Location = new Point(this.ClientSize.Width - 390, 50),
                BackColor = Color.FromArgb(189, 189, 189),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnOrders.FlatAppearance.BorderSize = 0;
            btnOrders.Click += (s, e) => ShowOrders();

            // Кнопка выхода
            var btnLogout = new Button
            {
                Text = "🚪 ВИХІД",
                Width = 100,
                Height = 40,
                Location = new Point(this.ClientSize.Width - 260, 50),
                BackColor = Color.FromArgb(160, 120, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                new LoginForm().Show();
                this.Close();
            };

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_btnCart);
            topPanel.Controls.Add(_cartCountLabel);
            topPanel.Controls.Add(btnOrders);
            topPanel.Controls.Add(btnLogout);

            // Панель каталога товаров
            _productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.FromArgb(250, 248, 240)
            };

            this.Controls.Add(_productsPanel);
            this.Controls.Add(topPanel);

            RefreshProducts();
        }

        private void RefreshProducts()
        {
            _productsPanel.Controls.Clear();
            List<Product> products;

            if (string.IsNullOrWhiteSpace(_searchBox.Text))
                products = ProductService.GetAllProducts();
            else
                products = ProductService.SearchProducts(_searchBox.Text);

            if (products.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "Товары не найдены",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    AutoSize = true
                };
                _productsPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var p in products)
            {
                var card = CreateProductCard(p);
                _productsPanel.Controls.Add(card);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Width = 270,
                Height = 480,
                Margin = new Padding(12),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(25, 118, 210), 3), 0, 0, card.Width - 1, card.Height - 1);
            };

            // Изображение товара
            var pic = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 270,
                Height = 220,
                BackColor = Color.FromArgb(240, 235, 225),
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.None
            };

            // Загрузка изображения
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var imageBytes = client.GetByteArrayAsync(product.ImagePath).Result;
                    var ms = new System.IO.MemoryStream(imageBytes);
                    pic.Image = Image.FromStream(ms);
                }
            }
            catch { }

            // Название товара
            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 230),
                MaximumSize = new Size(250, 50),
                ForeColor = Color.FromArgb(25, 25, 25)
            };

            // Бренд
            var lblBrand = new Label
            {
                Text = product.Brand,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(10, 285)
            };

            // Рейтинг
            var starsText = new string('★', product.Rating) + new string('☆', 5 - product.Rating);
            var lblRating = new Label
            {
                Text = starsText,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(10, 305)
            };

            // Размеры
            var lblSizes = new Label
            {
                Text = $"Розміри: {product.AvailableSizes}",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Location = new Point(10, 325),
                MaximumSize = new Size(250, 30)
            };

            // Цена
            var lblPrice = new Label
            {
                Text = $"{product.Price:C}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(10, 355)
            };

            // Кнопка "Подробнее"
            var btnDetails = new Button
            {
                Text = "ℹ ДЕТАЛЬНО",
                Width = 250,
                Height = 35,
                Location = new Point(10, 390),
                BackColor = Color.FromArgb(120, 100, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnDetails.FlatAppearance.BorderSize = 0;
            btnDetails.Click += (s, e) =>
            {
                var detailsForm = new ProductDetailsForm(product, _user, (p, size) =>
                {
                    AddToCart(p, size);
                });
                detailsForm.ShowDialog();
            };

            // Кнопка "В корзину"
            var btnAdd = new Button
            {
                Text = "🛒 У КОШИК",
                Width = 250,
                Height = 35,
                Location = new Point(10, 430),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddToCart(product);

            card.Controls.AddRange(new Control[] { pic, lblName, lblBrand, lblRating, lblSizes, lblPrice, btnDetails, btnAdd });
            return card;
        }

        private void AddToCart(Product product, string size = "M")
        {
            var existing = _cart.FirstOrDefault(ci => ci.Product.Id == product.Id && ci.Size == size);
            if (existing != null)
                existing.Quantity++;
            else
                _cart.Add(new CartItem { Product = product, Quantity = 1, Size = size });

            UpdateCartButton();
            
            // Обновляем открытую корзину если она есть
            if (_currentCartForm != null && !_currentCartForm.IsDisposed)
            {
                _currentCartForm.RefreshUI(_cart);
            }
            
            MessageBox.Show($"{product.Name} (Розмір: {size}) додано до кошика!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCartButton()
        {
            int totalItems = _cart.Sum(ci => ci.Quantity);
            if (totalItems > 0)
            {
                _cartCountLabel.Visible = true;
                _cartCountLabel.Text = totalItems > 99 ? "99+" : totalItems.ToString();
                _btnCart.Text = "🛒 Корзина";
            }
            else
            {
                _cartCountLabel.Visible = false;
                _btnCart.Text = "🛒 Корзина";
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
            var userOrders = OrderService.GetUserOrders(_user.Email);
            var ordersForm = new Form
            {
                Text = "Мої замовлення",
                Size = new Size(900, 650),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var topPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(25, 118, 210)
            };

            var lblTitle = new Label
            {
                Text = "📋 ІСТОРІЯ ЗАМОВЛЕНЬ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 12),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);

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
                        Width = 850,
                        Height = 130,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 10),
                        BackColor = Color.FromArgb(245, 245, 250)
                    };

                    panel.Controls.Add(new Label
                    {
                        Text = $"Замовлення №{order.Id}",
                        Location = new Point(10, 10),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.FromArgb(25, 118, 210)
                    });

                    panel.Controls.Add(new Label
                    {
                        Text = $"Від {order.OrderDate:dd.MM.yyyy HH:mm}",
                        Location = new Point(10, 35),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.Gray
                    });

                    var itemsText = string.Join(", ", order.Items.Select(i => $"{i.Product.Name} ×{i.Quantity}"));
                    panel.Controls.Add(new Label
                    {
                        Text = $"Товари: {itemsText}",
                        Location = new Point(10, 55),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9),
                        MaximumSize = new Size(830, 40)
                    });

                    panel.Controls.Add(new Label
                    {
                        Text = $"Сума: {order.TotalPrice:C}",
                        Location = new Point(10, 100),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = Color.FromArgb(244, 67, 54)
                    });

                    var statusColor = order.Status == "Доставлено" ? Color.Green :
                                     order.Status == "Відправлено" ? Color.Orange : Color.Gray;
                    panel.Controls.Add(new Label
                    {
                        Text = $"Статус: {order.Status}",
                        Location = new Point(700, 100),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = statusColor
                    });

                    flow.Controls.Add(panel);
                }
            }

            ordersForm.Controls.Add(flow);
            ordersForm.Controls.Add(topPanel);
            ordersForm.ShowDialog();
        }
    }
}
