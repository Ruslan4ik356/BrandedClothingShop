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

        public CatalogForm(User user)
        {
            _user = user;
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            this.Text = $"Каталог — { _user.FullName }";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Верхнє меню
            var topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            var lblTitle = new Label
            {
                Text = "BRANDED CLOTHING SHOP",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 10)
            };

            // Поле пошуку
            _searchBox = new TextBox
            {
                Width = 250,
                Height = 30,
                Location = new Point(20, 40),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Пошук за назвою або брендом..."
            };
            _searchBox.TextChanged += (s, e) => RefreshProducts();

            var btnCart = new Button
            {
                Text = $"🛒 Кошик (0)",
                Width = 120,
                Height = 36,
                BackColor = Color.FromArgb(255, 193, 7),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 140, 10)
            };
            btnCart.FlatAppearance.BorderSize = 0;
            btnCart.Click += (s, e) => ShowCart();

            var btnOrders = new Button
            {
                Text = "📋 Замовлення",
                Width = 120,
                Height = 36,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 270, 10)
            };
            btnOrders.FlatAppearance.BorderSize = 0;
            btnOrders.Click += (s, e) => ShowOrders();

            // Оновлення тексту кнопки кошика
            var updateCartButton = new Action(() =>
            {
                int totalItems = _cart.Sum(ci => ci.Quantity);
                btnCart.Text = totalItems == 0 ? "🛒 Кошик" : $"🛒 Кошик ({totalItems})";
            });

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(btnCart);
            topPanel.Controls.Add(btnOrders);

            // Каталог товарів
            _productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            this.Controls.Add(topPanel);
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

            if (string.IsNullOrWhiteSpace(_searchBox.Text))
                products = ProductService.GetAllProducts();
            else
                products = ProductService.SearchProducts(_searchBox.Text);

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
                Width = 220,
                Height = 320,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var pic = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 220,
                Height = 150,
                BackColor = Color.LightGray,
                Dock = DockStyle.Top
            };

            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 160)
            };

            var lblBrand = new Label
            {
                Text = product.Brand,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, 180)
            };

            var lblPrice = new Label
            {
                Text = $"{product.Price:C}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 0),
                AutoSize = true,
                Location = new Point(10, 200)
            };

            var btnAdd = new Button
            {
                Text = "🛒 Додати в кошик",
                Width = 200,
                Height = 36,
                Location = new Point(10, 240),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => addToCartAction();

            card.Controls.AddRange(new Control[] { pic, lblName, lblBrand, lblPrice, btnAdd });
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
    }
}