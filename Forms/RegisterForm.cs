using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
        
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 248, 250);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                ColumnCount = 1,
                RowCount = 9,
                AutoSize = true
            };

            // Верхняя панель с титулом
            var topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(79, 89, 44),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = "📝 Реєстрація",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            topPanel.Controls.Add(lblTitle);

            var lblName = new Label 
            { 
                Text = "Повне ім'я:", 
                AutoSize = true,
                ForeColor = Color.FromArgb(79, 89, 44),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var txtName = new TextBox 
            { 
                Width = 320, 
                Height = 30,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblEmail = new Label 
            { 
                Text = "Електронна пошта:", 
                AutoSize = true,
                ForeColor = Color.FromArgb(79, 89, 44),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var txtEmail = new TextBox 
            { 
                Width = 320, 
                Height = 30,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblPass = new Label 
            { 
                Text = "Пароль:", 
                AutoSize = true,
                ForeColor = Color.FromArgb(79, 89, 44),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var txtPass = new TextBox 
            { 
                Width = 320, 
                Height = 30, 
                PasswordChar = '*',
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnReg = new Button
            {
                Text = "✓ Зареєструватися",
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(200, 242, 48),
                ForeColor = Color.FromArgb(79, 89, 44),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnReg.FlatAppearance.BorderSize = 0;
            btnReg.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    MessageBox.Show("Усі поля обов’язкові.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = new User
                {
                    FullName = txtName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Password = txtPass.Text
                };

                if (UserService.RegisterUser(user))
                {
                    MessageBox.Show("Реєстрацію успішно завершено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new LoginForm().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Користувач з такою поштою вже існує.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var btnBack = new LinkLabel
            {
                Text = "← Повернутися до входу",
                LinkColor = Color.FromArgb(79, 89, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10)
            };
            btnBack.LinkClicked += (s, e) =>
            {
                new LoginForm().Show();
                this.Close();
            };

            panel.Controls.Add(topPanel);
            panel.Controls.Add(lblName);
            panel.Controls.Add(txtName);
            panel.Controls.Add(lblEmail);
            panel.Controls.Add(txtEmail);
            panel.Controls.Add(lblPass);
            panel.Controls.Add(txtPass);
            panel.Controls.Add(btnReg);
            panel.Controls.Add(btnBack);

            this.Controls.Add(panel);
        }
    }
}