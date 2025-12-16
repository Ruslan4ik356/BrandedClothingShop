using BrandedClothingShop.Models;
using BrandedClothingShop.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrandedClothingShop.Forms
{
    public class ReviewForm : Form
    {
        private readonly Product _product;
        private readonly User _user;
        private int _selectedRating = 5;
        private Label _ratingLabel = null!;

        public ReviewForm(Product product, User user)
        {
            _product = product;
            _user = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Додати відзив - {_product.Name}";
            this.Size = new Size(600, 500);
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
                Text = "ДОДАТИ ВІДЗИВ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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

            // Рейтинг
            var lblRating = new Label
            {
                Text = "Оцінка:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };

            _ratingLabel = new Label
            {
                Text = "★★★★★",
                Font = new Font("Segoe UI", 20),
                ForeColor = Color.FromArgb(255, 152, 0),
                Location = new Point(0, 30),
                AutoSize = true
            };

            var ratingPanel = new Panel
            {
                Location = new Point(0, 60),
                Width = 300,
                Height = 50,
                BorderStyle = BorderStyle.FixedSingle
            };

            for (int i = 1; i <= 5; i++)
            {
                var btnStar = new Button
                {
                    Text = "★",
                    Width = 50,
                    Height = 40,
                    Location = new Point((i - 1) * 50, 5),
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(255, 152, 0),
                    Font = new Font("Segoe UI", 24),
                    FlatStyle = FlatStyle.Flat,
                    Tag = i
                };
                btnStar.FlatAppearance.BorderSize = 0;
                int rating = i;
                btnStar.Click += (s, e) =>
                {
                    _selectedRating = rating;
                    UpdateRatingDisplay();
                };
                ratingPanel.Controls.Add(btnStar);
            }

            mainPanel.Controls.Add(lblRating);
            mainPanel.Controls.Add(_ratingLabel);
            mainPanel.Controls.Add(ratingPanel);

            // Текст відзиву
            var lblComment = new Label
            {
                Text = "Ваш відзив:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(0, 130),
                AutoSize = true
            };

            var txtComment = new TextBox
            {
                Width = 540,
                Height = 150,
                Location = new Point(0, 160),
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            mainPanel.Controls.Add(lblComment);
            mainPanel.Controls.Add(txtComment);

            // Нижняя панель
            var bottomPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            var btnSubmit = new Button
            {
                Text = "НАДІСЛАТИ ВІДЗИВ",
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(350, 15)
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtComment.Text))
                {
                    MessageBox.Show("Будь ласка, напишіть відзив!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ReviewService.AddReview(_product.Id, _user.Email, _user.FullName, _selectedRating, txtComment.Text.Trim());
                MessageBox.Show("✅ Відзив успішно додано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Location = new Point(200, 15)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnSubmit);
            bottomPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
        }

        private void UpdateRatingDisplay()
        {
            string stars = new string('★', _selectedRating) + new string('☆', 5 - _selectedRating);
            _ratingLabel.Text = stars;
        }
    }
}
