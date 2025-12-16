using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class CartForm : Form
    {
        private readonly User _user;
        private List<CartItem> _cart;
        private Action<List<CartItem>> _onCartChanged;
        private FlowLayoutPanel _itemsPanel = null!;
        private Label _totalLabel = null!;

        public CartForm(User user, List<CartItem> cart, Action<List<CartItem>> onCartChanged)
        {
            _user = user;
            _cart = cart;
            _onCartChanged = onCartChanged;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Мій кошик";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Верхняя панель
            var topPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Black
            };

            var lblTitle = new Label
            {
                Text = "МІЙ КОШИК",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);

            // Панель со товарами
            _itemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 248, 240),
                AutoScroll = true,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false
            };

            RefreshCart();

            // Нижняя панель с итогом и кнопками
            var bottomPanel = new Panel
            {
                Height = 120,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            _totalLabel = new Label
            {
                Text = $"Всього: 0.00₴",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(20, 10),
                AutoSize = true
            };

            var btnCheckout = new Button
            {
                Text = "ОФОРМИТИ ЗАМОВЛЕННЯ",
                Width = 230,
                Height = 40,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(500, 40)
            };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += (s, e) =>
            {
                if (_cart.Count > 0)
                {
                    // Відкриваємо форму оформлення замовлення
                    var checkoutForm = new CheckoutForm(_user, _cart);
                    if (checkoutForm.ShowDialog() == DialogResult.OK)
                    {
                        // Замовлення успішно оформлено
                        _cart.Clear();
                        _onCartChanged(_cart);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Ваш кошик порожній!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            var btnContinueShopping = new Button
            {
                Text = "ПРОДОВЖИТИ ПОКУПКИ",
                Width = 230,
                Height = 40,
                BackColor = Color.FromArgb(51, 51, 51),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(260, 40)
            };
            btnContinueShopping.FlatAppearance.BorderSize = 0;
            btnContinueShopping.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(_totalLabel);
            bottomPanel.Controls.Add(btnCheckout);
            bottomPanel.Controls.Add(btnContinueShopping);

            this.Controls.Add(_itemsPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);

            UpdateTotal();
        }

        public void RefreshUI(List<CartItem> updatedCart)
        {
            _cart = updatedCart;
            RefreshCart();
            UpdateTotal();
        }

        private void RefreshCart()
        {
            _itemsPanel.Controls.Clear();

            if (_cart.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "Кошик порожній\n\nДодайте товари для оформлення замовлення",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                _itemsPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var item in _cart)
            {
                var itemPanel = CreateCartItemPanel(item);
                itemPanel.Margin = new Padding(0, 0, 0, 15);
                _itemsPanel.Controls.Add(itemPanel);
            }
        }

        private Panel CreateCartItemPanel(CartItem item)
        {
            var panel = new Panel
            {
                Width = 800,
                Height = 100,
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 15)
            };

            var itemPrice = item.Product.Price * item.Quantity;

            // Миниатюра
            var picBox = new PictureBox
            {
                Width = 90,
                Height = 90,
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(225, 245, 254)
            };

            try
            {
                // Генерируем цветное изображение на основе ID товара
                var bitmap = new Bitmap(90, 90);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Разные цвета для разных товаров
                    Color[] colors = new[]
                    {
                        Color.FromArgb(33, 150, 243),  // Синий
                        Color.FromArgb(76, 175, 80),   // Зелёный
                        Color.FromArgb(244, 67, 54),   // Красный
                        Color.FromArgb(233, 30, 99),   // Розовый
                        Color.FromArgb(255, 152, 0),   // Оранжевый
                        Color.FromArgb(156, 39, 176),  // Фиолетовый
                        Color.FromArgb(63, 81, 181),   // Тёмно-синий
                        Color.FromArgb(0, 150, 136),   // Бирюзовый
                        Color.FromArgb(255, 193, 7),   // Жёлтый
                        Color.FromArgb(139, 69, 19),   // Коричневый
                        Color.FromArgb(96, 125, 139),  // Синевато-серый
                        Color.FromArgb(0, 0, 0)        // Чёрный
                    };

                    var bgColor = colors[Math.Min(item.Product.Id - 1, colors.Length - 1)];
                    graphics.Clear(bgColor);
                }

                picBox.Image = bitmap;
            }
            catch
            {
                picBox.BackColor = Color.FromArgb(220, 220, 220);
            }

            // Информация
            var lblName = new Label
            {
                Text = item.Product.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(105, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(25, 25, 25)
            };

            var lblBrand = new Label
            {
                Text = $"{item.Product.Brand} | Розмір: {item.Size}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(105, 32),
                AutoSize = true
            };

            var lblPrice = new Label
            {
                Text = $"{item.Product.Price:C} × {item.Quantity}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(105, 50),
                AutoSize = true
            };

            // Количество
            var btnMinus = new Button
            {
                Text = "-",
                Width = 30,
                Height = 28,
                Location = new Point(390, 60),
                BackColor = Color.FromArgb(255, 152, 0),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnMinus.Click += (s, e) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    _onCartChanged(_cart);
                    RefreshCart();
                    UpdateTotal();
                }
            };

            var lblQty = new Label
            {
                Text = item.Quantity.ToString(),
                Font = new Font("Segoe UI", 11),
                Location = new Point(430, 63),
                Width = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnPlus = new Button
            {
                Text = "+",
                Width = 30,
                Height = 28,
                Location = new Point(470, 60),
                BackColor = Color.FromArgb(213, 242, 109),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnPlus.Click += (s, e) =>
            {
                item.Quantity++;
                _onCartChanged(_cart);
                RefreshCart();
                UpdateTotal();
            };

            // Сумма
            var lblTotal = new Label
            {
                Text = $"{itemPrice:C}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 242, 48),
                Location = new Point(520, 63),
                AutoSize = true
            };

            // Видалити
            var btnRemove = new Button
            {
                Text = "✕",
                Width = 35,
                Height = 35,
                Location = new Point(750, 30),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14)
            };
            btnRemove.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    $"Видалити '{item.Product.Name}' з кошика?",
                    "Видалення товара",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result == DialogResult.Yes)
                {
                    _cart.Remove(item);
                    _onCartChanged(_cart);
                    RefreshCart();
                    UpdateTotal();
                    MessageBox.Show($"'{item.Product.Name}' видалено з кошика!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            panel.Controls.Add(picBox);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblBrand);
            panel.Controls.Add(lblPrice);
            panel.Controls.Add(btnMinus);
            panel.Controls.Add(lblQty);
            panel.Controls.Add(btnPlus);
            panel.Controls.Add(lblTotal);
            panel.Controls.Add(btnRemove);

            return panel;
        }

        private void UpdateTotal()
        {
            var total = _cart.Sum(i => i.Product.Price * i.Quantity);
            _totalLabel.Text = $"Всього: {total:C}";
        }
    }
}
