using BrandedClothingShop.Services;
using BrandedClothingShop.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "BRANDED — Вхід";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Верхняя панель с логотипом
            var topPanel = new Panel
            {
                Height = 100,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };

            var lblLogo = new Label
            {
                Text = "BRANDED",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            topPanel.Controls.Add(lblLogo);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 30, 40, 40),
                ColumnCount = 1,
                RowCount = 7,
                AutoSize = true
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Увійти",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var lblEmail = new Label 
            { 
                Text = "Електронна пошта", 
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var txtEmail = new TextBox 
            { 
                Width = 300, 
                Height = 35,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            var lblPass = new Label 
            { 
                Text = "Пароль", 
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var txtPass = new TextBox 
            { 
                Width = 300, 
                Height = 35, 
                PasswordChar = '*',
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            var btnLogin = new Button
            {
                Text = "УВІЙТИ",
                Width = 300,
                Height = 40,
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) =>
            {
                var user = UserService.Authenticate(txtEmail.Text.Trim(), txtPass.Text);
                if (user != null)
                {
                    MessageBox.Show($"Ласкаво просимо, {user.FullName}!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var catalog = new CatalogFormModern(user);
                    catalog.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Невірна електронна пошта або пароль.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var btnRegister = new LinkLabel
            {
                Text = "Немаєте облікового запису? Зареєстуватися",
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = Color.Black
            };
            btnRegister.LinkClicked += (s, e) =>
            {
                new RegisterForm().Show();
                this.Hide();
            };

            // Додаємо в panel
            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblEmail);
            panel.Controls.Add(txtEmail);
            panel.Controls.Add(lblPass);
            panel.Controls.Add(txtPass);
            panel.Controls.Add(btnLogin);
            panel.Controls.Add(btnRegister);

            // Додаємо в форму
            this.Controls.Add(panel);
            this.Controls.Add(topPanel);
        }
    }
}