using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class CheckoutForm : Form
    {
        private readonly User _user;
        private readonly List<CartItem> _cart;
        private decimal _subtotal;
        private decimal _shippingCost;
        private string _selectedShippingMethod = "Standard";

        private Label _totalLabel = null!;
        private Label _shippingLabel = null!;
        private Label _subtotalLabel = null!;
        private ComboBox _shippingCombo = null!;
        private TextBox _nameInput = null!;
        private TextBox _addressInput = null!;
        private TextBox _cityInput = null!;
        private TextBox _postalInput = null!;
        private TextBox _phoneInput = null!;

        public CheckoutForm(User user, List<CartItem> cart)
        {
            _user = user;
            _cart = new List<CartItem>(cart);
            _subtotal = _cart.Sum(i => i.Product.Price * i.Quantity);
            InitializeComponent();
            LoadUserData();
        }

        private void InitializeComponent()
        {
            this.Text = "Оформлення замовлення";
            this.Size = new Size(900, 750);
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
                Text = "ОФОРМЛЕННЯ ЗАМОВЛЕННЯ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);

            // Основная панель
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30)
            };

            // Розділ адреси доставки
            var lblDelivery = new Label
            {
                Text = "АДРЕСА ДОСТАВКИ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var lblName = new Label { Text = "ПІБ:", Font = new Font("Segoe UI", 10), Location = new Point(0, 35), AutoSize = true };
            _nameInput = new TextBox { Width = 350, Location = new Point(120, 32), Text = _user.PhoneNumber };

            var lblAddress = new Label { Text = "Адреса:", Font = new Font("Segoe UI", 10), Location = new Point(0, 70), AutoSize = true };
            _addressInput = new TextBox { Width = 350, Location = new Point(120, 67), Text = _user.Address };

            var lblCity = new Label { Text = "Місто:", Font = new Font("Segoe UI", 10), Location = new Point(0, 105), AutoSize = true };
            _cityInput = new TextBox { Width = 350, Location = new Point(120, 102), Text = _user.City };

            var lblPostal = new Label { Text = "Індекс:", Font = new Font("Segoe UI", 10), Location = new Point(0, 140), AutoSize = true };
            _postalInput = new TextBox { Width = 350, Location = new Point(120, 137), Text = _user.PostalCode };

            var lblPhone = new Label { Text = "Телефон:", Font = new Font("Segoe UI", 10), Location = new Point(0, 175), AutoSize = true };
            _phoneInput = new TextBox { Width = 350, Location = new Point(120, 172), Text = _user.PhoneNumber };

            mainPanel.Controls.Add(lblDelivery);
            mainPanel.Controls.Add(lblName);
            mainPanel.Controls.Add(_nameInput);
            mainPanel.Controls.Add(lblAddress);
            mainPanel.Controls.Add(_addressInput);
            mainPanel.Controls.Add(lblCity);
            mainPanel.Controls.Add(_cityInput);
            mainPanel.Controls.Add(lblPostal);
            mainPanel.Controls.Add(_postalInput);
            mainPanel.Controls.Add(lblPhone);
            mainPanel.Controls.Add(_phoneInput);

            // Секция способа доставки
            var lblShipping = new Label
            {
                Text = "СПОСІБ ДОСТАВКИ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 220),
                AutoSize = true
            };

            _shippingCombo = new ComboBox
            {
                Width = 350,
                Height = 30,
                Location = new Point(0, 250),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            _shippingCombo.Items.Add("Стандартна доставка - 49.99 грн (2-3 дня)");
            _shippingCombo.Items.Add("Експрес доставка - 99.99 грн (1 день)");
            _shippingCombo.Items.Add("Самовивіз - Безкоштовно");
            _shippingCombo.SelectedIndex = 0;
            _shippingCombo.SelectedIndexChanged += (s, e) =>
            {
                _selectedShippingMethod = _shippingCombo.SelectedIndex == 0 ? "Standard" :
                                         _shippingCombo.SelectedIndex == 1 ? "Express" : "Pickup";
                UpdateShippingCost();
            };

            mainPanel.Controls.Add(lblShipping);
            mainPanel.Controls.Add(_shippingCombo);

            // Секция товаров
            var lblItems = new Label
            {
                Text = "ВАШІ ТОВАРИ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 310),
                AutoSize = true
            };

            var itemsPanel = new Panel
            {
                Location = new Point(0, 340),
                Width = 850,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250),
                AutoScroll = true
            };

            int yPos = 5;
            foreach (var item in _cart)
            {
                var itemLabel = new Label
                {
                    Text = $"{item.Product.Name} (Розмір: {item.Size}) × {item.Quantity} = {item.Product.Price * item.Quantity:C}",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(10, yPos),
                    Width = 800,
                    AutoSize = true
                };
                itemsPanel.Controls.Add(itemLabel);
                yPos += 25;
            }

            mainPanel.Controls.Add(lblItems);
            mainPanel.Controls.Add(itemsPanel);

            // Секция итога
            _subtotalLabel = new Label
            {
                Text = $"Сума товарів: {_subtotal:C}",
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 480),
                AutoSize = true,
                ForeColor = Color.Black
            };

            _shippingLabel = new Label
            {
                Text = $"Доставка: 49.99 грн",
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 510),
                AutoSize = true,
                ForeColor = Color.Black
            };

            _totalLabel = new Label
            {
                Text = $"ВСЬОГО: {_subtotal + OrderService.GetShippingCost(_subtotal, "Standard"):C}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(0, 545),
                AutoSize = true,
                ForeColor = Color.FromArgb(229, 57, 53)
            };

            mainPanel.Controls.Add(_subtotalLabel);
            mainPanel.Controls.Add(_shippingLabel);
            mainPanel.Controls.Add(_totalLabel);

            // Нижняя панель с кнопками
            var bottomPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            var btnConfirm = new Button
            {
                Text = "ПІДТВЕРДИТИ ЗАМОВЛЕННЯ",
                Width = 350,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(500, 20)
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += (s, e) => ConfirmOrder();

            var btnCancel = new Button
            {
                Text = "СКАСУВАТИ",
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(260, 20)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnConfirm);
            bottomPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
        }

        private void LoadUserData()
        {
            if (!string.IsNullOrEmpty(_user.PhoneNumber))
                _nameInput.Text = _user.PhoneNumber;
            if (!string.IsNullOrEmpty(_user.Address))
                _addressInput.Text = _user.Address;
            if (!string.IsNullOrEmpty(_user.City))
                _cityInput.Text = _user.City;
            if (!string.IsNullOrEmpty(_user.PostalCode))
                _postalInput.Text = _user.PostalCode;
        }

        private void UpdateShippingCost()
        {
            _shippingCost = OrderService.GetShippingCost(_subtotal, _selectedShippingMethod);
            _shippingLabel.Text = $"Доставка: {_shippingCost:C}";
            _totalLabel.Text = $"ВСЬОГО: {_subtotal + _shippingCost:C}";
        }

        private void ConfirmOrder()
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(_nameInput.Text))
            {
                MessageBox.Show("Будь ласка, введіть ПІБ", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_addressInput.Text))
            {
                MessageBox.Show("Будь ласка, введіть адресу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_cityInput.Text))
            {
                MessageBox.Show("Будь ласка, введіть місто", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_phoneInput.Text))
            {
                MessageBox.Show("Будь ласка, введіть телефон", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Создание заказа
            var order = OrderService.CreateOrder(
                _user.Email,
                _cart,
                _addressInput.Text,
                _cityInput.Text,
                _postalInput.Text,
                _phoneInput.Text,
                _nameInput.Text,
                _selectedShippingMethod
            );

            if (order != null)
            {
                MessageBox.Show(
                    $"✅ Замовлення #{order.Id} успішно оформлено!\n\n" +
                    $"Сума товарів: {order.SubTotal:C}\n" +
                    $"Доставка: {order.ShippingCost:C}\n" +
                    $"ВСЬОГО: {order.TotalPrice:C}\n\n" +
                    $"Дата доставки: {(order.ShippingMethod == "Express" ? "Завтра" : order.ShippingMethod == "Standard" ? "За 2-3 дня" : "Самовивіз")}\n" +
                    $"Адреса: {_cityInput.Text}, {_addressInput.Text}",
                    "Успішно",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
