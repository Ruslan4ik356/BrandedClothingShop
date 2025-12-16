using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class OrderHistoryForm : Form
    {
        private readonly User _user;
        private FlowLayoutPanel _ordersPanel = null!;

        public OrderHistoryForm(User user)
        {
            _user = user;
            InitializeComponent();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.Text = "Історія замовлень";
            this.Size = new Size(1000, 700);
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
                Text = "ІСТОРІЯ ЗАМОВЛЕНЬ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);

            // Панель зі списком замовлень
            _ordersPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 248, 240),
                AutoScroll = true,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false
            };

            this.Controls.Add(_ordersPanel);
            this.Controls.Add(topPanel);
        }

        private void LoadOrders()
        {
            _ordersPanel.Controls.Clear();

            var orders = OrderService.GetUserOrders(_user.Email);

            if (orders.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "У вас ще немає замовлень\n\nПочніть покупки тепер!",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Width = 900,
                    Height = 200,
                    Dock = DockStyle.Fill
                };
                _ordersPanel.Controls.Add(emptyLabel);
                return;
            }

            // Сортируем по дате (новые в начале)
            orders = orders.OrderByDescending(o => o.OrderDate).ToList();

            foreach (var order in orders)
            {
                var orderCard = CreateOrderCard(order);
                orderCard.Margin = new Padding(0, 0, 0, 15);
                _ordersPanel.Controls.Add(orderCard);
            }
        }

        private Panel CreateOrderCard(Order order)
        {
            var card = new Panel
            {
                Width = 900,
                Height = 180,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Статус-бар (вверху)
            var statusColor = order.Status switch
            {
                "Обробляється" => Color.FromArgb(255, 152, 0),    // Оранжевый
                "Відправлено" => Color.FromArgb(33, 150, 243),    // Синий
                "Доставлено" => Color.FromArgb(76, 175, 80),      // Зелёный
                "Скасовано" => Color.FromArgb(244, 67, 54),       // Красный
                _ => Color.Gray
            };

            var statusPanel = new Panel
            {
                Width = 900,
                Height = 8,
                Location = new Point(0, 0),
                BackColor = statusColor
            };

            // Номер і дата замовлення
            var lblOrderId = new Label
            {
                Text = $"Замовлення #{order.Id}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true,
                ForeColor = Color.Black
            };

            var lblDate = new Label
            {
                Text = $"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(15, 38),
                AutoSize = true
            };

            // Статус
            var lblStatus = new Label
            {
                Text = $"Статус: {order.Status}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = statusColor,
                Location = new Point(400, 15),
                AutoSize = true
            };

            // Спосіб доставки і дата доставки
            var shippingText = order.ShippingMethod switch
            {
                "Express" => "Експрес-доставка (1 день)",
                "Pickup" => "Самовивіз",
                _ => "Стандартна доставка (2-3 дня)"
            };

            var lblShipping = new Label
            {
                Text = shippingText,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(400, 38),
                AutoSize = true
            };

            // Адреса доставки
            var lblAddress = new Label
            {
                Text = $"Адреса: {order.DeliveryCity}, {order.DeliveryAddress}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(15, 60),
                Width = 850,
                AutoSize = true,
                AutoEllipsis = true
            };

            // Товари (компактный список)
            var itemsText = string.Join(", ", order.Items.Select(i => $"{i.Product.Name} (×{i.Quantity})"));
            var lblItems = new Label
            {
                Text = $"Товари: {itemsText}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(15, 85),
                Width = 850,
                Height = 35,
                AutoSize = false,
                AutoEllipsis = true
            };

            // Цена
            var lblPrice = new Label
            {
                Text = $"Сума товарів: {order.SubTotal:C}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 125),
                AutoSize = true
            };

            var lblShippingCost = new Label
            {
                Text = $"Доставка: {order.ShippingCost:C}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 125),
                AutoSize = true
            };

            var lblTotal = new Label
            {
                Text = $"ВСЬОГО: {order.TotalPrice:C}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(229, 57, 53),
                Location = new Point(500, 120),
                AutoSize = true
            };

            // Кнопка деталей
            var btnDetails = new Button
            {
                Text = "Деталі",
                Width = 100,
                Height = 28,
                Location = new Point(780, 125),
                BackColor = Color.FromArgb(51, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnDetails.FlatAppearance.BorderSize = 0;
            btnDetails.Click += (s, e) => ShowOrderDetails(order);

            // Кнопка повторення замовлення (якщо статус не "Скасовано")
            if (order.Status != "Скасовано")
            {
                var btnReorder = new Button
                {
                    Text = "Повторити",
                    Width = 100,
                    Height = 28,
                    Location = new Point(665, 125),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9)
                };
                btnReorder.FlatAppearance.BorderSize = 0;
                btnReorder.Click += (s, e) => ReorderItems(order);
                card.Controls.Add(btnReorder);

                // Кнопка скасування замовлення (якщо статус "Обробляється")
                if (order.Status == "Обробляється")
                {
                    var btnCancel = new Button
                    {
                        Text = "Скасувати",
                        Width = 100,
                        Height = 28,
                        Location = new Point(550, 125),
                        BackColor = Color.FromArgb(244, 67, 54),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9)
                    };
                    btnCancel.FlatAppearance.BorderSize = 0;
                    btnCancel.Click += (s, e) => CancelOrder(order);
                    card.Controls.Add(btnCancel);
                }
            }

            card.Controls.Add(statusPanel);
            card.Controls.Add(lblOrderId);
            card.Controls.Add(lblDate);
            card.Controls.Add(lblStatus);
            card.Controls.Add(lblShipping);
            card.Controls.Add(lblAddress);
            card.Controls.Add(lblItems);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblShippingCost);
            card.Controls.Add(lblTotal);
            card.Controls.Add(btnDetails);

            return card;
        }

        private void ShowOrderDetails(Order order)
        {
            var detailsText = $"Замовлення #{order.Id}\n\n" +
                $"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}\n" +
                $"Статус: {order.Status}\n\n" +
                $"АДРЕСА ДОСТАВКИ:\n" +
                $"ПІБ: {order.DeliveryName}\n" +
                $"Адреса: {order.DeliveryAddress}\n" +
                $"Місто: {order.DeliveryCity}\n" +
                $"Індекс: {order.DeliveryPostalCode}\n" +
                $"Телефон: {order.DeliveryPhone}\n\n" +
                $"СПОСОБ ДОСТАВКИ: {order.ShippingMethod}\n\n" +
                $"ТОВАРИ:\n";

            foreach (var item in order.Items)
            {
                detailsText += $"  • {item.Product.Name} (Розмір: {item.Size}) × {item.Quantity} = {item.Product.Price * item.Quantity:C}\n";
            }

            detailsText += $"\nСума товарів: {order.SubTotal:C}\n" +
                $"Доставка: {order.ShippingCost:C}\n" +
                $"ВСЬОГО: {order.TotalPrice:C}";

            MessageBox.Show(detailsText, $"Деталі замовлення #{order.Id}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReorderItems(Order order)
        {
            var result = MessageBox.Show(
                $"Додати все товари з замовлення #{order.Id} до кошика?",
                "Повторити замовлення",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("✅ Товари добавлені до кошика!\n\nПерейдіть до кошика для оформлення нового замовлення", 
                    "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void CancelOrder(Order order)
        {
            var result = MessageBox.Show(
                $"Ви впевнені, що хочете скасувати замовлення #{order.Id}?\n\nЦю дію не можна скасувати.",
                "Скасування замовлення",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                // Оновлюємо статус замовлення
                OrderService.UpdateOrderStatus(order.Id, "Скасовано");
                
                MessageBox.Show("✅ Замовлення успішно скасовано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Перезавантажуємо список замовлень
                LoadOrders();
            }
        }
    }
}
