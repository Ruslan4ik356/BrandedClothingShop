using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class UserProfileForm : Form
    {
        private readonly User _user;
        private TextBox txtName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPhone = null!;
        private TextBox txtAddress = null!;
        private TextBox txtCity = null!;
        private TextBox txtPostalCode = null!;

        public UserProfileForm(User user)
        {
            _user = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Мій профіль";
            this.Size = new Size(600, 700);
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
                Text = "МІЙ ПРОФІЛЬ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);

            // Основна панель
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30)
            };

            // ПІБ
            var lblName = new Label { Text = "Повне ім'я:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 0), AutoSize = true };
            txtName = new TextBox { Width = 300, Location = new Point(0, 25), Text = _user.FullName, BorderStyle = BorderStyle.FixedSingle };

            // Email (читається лише)
            var lblEmail = new Label { Text = "Електронна пошта:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 65), AutoSize = true };
            txtEmail = new TextBox { Width = 300, Location = new Point(0, 90), Text = _user.Email, BorderStyle = BorderStyle.FixedSingle, ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };

            // Телефон
            var lblPhone = new Label { Text = "Телефон:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 130), AutoSize = true };
            txtPhone = new TextBox { Width = 300, Location = new Point(0, 155), Text = _user.PhoneNumber ?? "", BorderStyle = BorderStyle.FixedSingle };

            // Адреса
            var lblAddress = new Label { Text = "Адреса:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 195), AutoSize = true };
            txtAddress = new TextBox { Width = 300, Location = new Point(0, 220), Text = _user.Address ?? "", BorderStyle = BorderStyle.FixedSingle };

            // Місто
            var lblCity = new Label { Text = "Місто:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 260), AutoSize = true };
            txtCity = new TextBox { Width = 300, Location = new Point(0, 285), Text = _user.City ?? "", BorderStyle = BorderStyle.FixedSingle };

            // Поштовий індекс
            var lblPostal = new Label { Text = "Поштовий індекс:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 325), AutoSize = true };
            txtPostalCode = new TextBox { Width = 300, Location = new Point(0, 350), Text = _user.PostalCode ?? "", BorderStyle = BorderStyle.FixedSingle };

            mainPanel.Controls.Add(lblName);
            mainPanel.Controls.Add(txtName);
            mainPanel.Controls.Add(lblEmail);
            mainPanel.Controls.Add(txtEmail);
            mainPanel.Controls.Add(lblPhone);
            mainPanel.Controls.Add(txtPhone);
            mainPanel.Controls.Add(lblAddress);
            mainPanel.Controls.Add(txtAddress);
            mainPanel.Controls.Add(lblCity);
            mainPanel.Controls.Add(txtCity);
            mainPanel.Controls.Add(lblPostal);
            mainPanel.Controls.Add(txtPostalCode);

            // Нижняя панель
            var bottomPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            var btnSave = new Button
            {
                Text = "ЗБЕРЕГТИ ЗМІНИ",
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(350, 20)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                // Оновлюємо дані користувача
                _user.FullName = txtName.Text;
                _user.PhoneNumber = txtPhone.Text;
                _user.Address = txtAddress.Text;
                _user.City = txtCity.Text;
                _user.PostalCode = txtPostalCode.Text;

                // Зберігаємо зміни
                UserService.UpdateUser(_user);

                MessageBox.Show("✅ Профіль успішно оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var btnCancel = new Button
            {
                Text = "СКАСУВАТИ",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(200, 20)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnSave);
            bottomPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
        }
    }
}
