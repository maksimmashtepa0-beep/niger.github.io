using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public partial class MainMenuForm : Form
    {
        private Button btnProducts;
        private Button btnIncoming;
        private Button btnAdjustment;
        private Button btnExpense;
        private Button btnReturns;
        private Button btnStatistics;
        private Button btnHistory;

        public MainMenuForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form_Paint;
            this.Load += MainMenuForm_Load;
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(9, 14, 24), Color.FromArgb(20, 28, 38), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            using (var pen = new Pen(Color.FromArgb(99, 125, 255), 1.5f))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
            }
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к SQL Server. Проверьте строку подключения в App.config и убедитесь, что база данных создана.", "Подключение к БД");
            }
        }

        private void InitializeComponent()
        {
            this.Text = "СкладАналитика";
            this.Size = new Size(560, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 14, 18);

            Panel headerPanel = new Panel
            {
                Location = new Point(20, 18),
                Size = new Size(500, 90),
                BackColor = Color.FromArgb(30, 36, 48),
                BorderStyle = BorderStyle.None
            };
            headerPanel.Region = UiHelpers.CreateRoundedRegion(headerPanel.Width, headerPanel.Height, 20);

            Label title = new Label
            {
                Text = "СкладАналитика",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 196, 94),
                Location = new Point(0, 18),
                Size = new Size(500, 34),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label subtitle = new Label
            {
                Text = "Панель управления складом",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 195, 218),
                Location = new Point(0, 54),
                Size = new Size(500, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Color accentGold = Color.FromArgb(245, 196, 94);
            Color accentGreen = Color.FromArgb(39, 174, 96);
            Color accentRed = Color.FromArgb(220, 88, 88);
            Color accentPurple = Color.FromArgb(130, 99, 255);

            btnProducts = CreateLuxuryButton("Товары", accentGold, new Point(130, 120), new Size(300, 56));
            btnProducts.Click += BtnProducts_Click;

            btnIncoming = CreateLuxuryButton("Приход", accentGreen, new Point(130, 190), new Size(300, 56));
            btnIncoming.Click += BtnIncoming_Click;

            btnAdjustment = CreateLuxuryButton("Корректировка", Color.FromArgb(56, 142, 168), new Point(130, 260), new Size(300, 56));
            btnAdjustment.Click += BtnAdjustment_Click;

            btnExpense = CreateLuxuryButton("Расход", accentRed, new Point(130, 330), new Size(300, 56));
            btnExpense.Click += BtnExpense_Click;

            btnReturns = CreateLuxuryButton("Возвраты", Color.FromArgb(202, 132, 55), new Point(130, 400), new Size(300, 56));
            btnReturns.Click += BtnReturns_Click;

            btnStatistics = CreateLuxuryButton("Статистика", accentPurple, new Point(130, 470), new Size(300, 56));
            btnStatistics.Click += BtnStatistics_Click;

            btnHistory = CreateLuxuryButton("История", Color.FromArgb(35, 110, 176), new Point(130, 540), new Size(300, 56));
            btnHistory.Click += BtnHistory_Click;

            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(subtitle);

            this.Controls.Add(headerPanel);
            this.Controls.Add(btnProducts);
            this.Controls.Add(btnIncoming);
            this.Controls.Add(btnAdjustment);
            this.Controls.Add(btnExpense);
            this.Controls.Add(btnReturns);
            this.Controls.Add(btnStatistics);
            this.Controls.Add(btnHistory);
        }

        private Button CreateLuxuryButton(string text, Color backColor, Point location, Size size)
        {
            var button = new Button
            {
                Text = text,
                Size = size,
                Location = location,
                BackColor = backColor,
                ForeColor = text == "Товары" ? Color.FromArgb(30, 20, 10) : Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.Tag = new[] { backColor, UiHelpers.DarkenColor(backColor, 18), text == "Товары" ? Color.FromArgb(30, 20, 10) : Color.White };
            button.Region = UiHelpers.CreateRoundedRegion(button.Width, button.Height, 12);
            button.Paint += (sender, e) =>
            {
                var btn = (Button)sender;
                var colors = (Color[])btn.Tag;
                using (var brush = new LinearGradientBrush(btn.ClientRectangle, colors[0], UiHelpers.DarkenColor(colors[0], 12), LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRoundedRectangle(brush, 0, 0, btn.Width, btn.Height, 12);
                }

                using (var pen = new Pen(Color.FromArgb(70, 255, 255, 255), 1f))
                {
                    e.Graphics.DrawRoundedRectangle(pen, 1, 1, btn.Width - 2, btn.Height - 2, 12);
                }

                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            };

            button.MouseEnter += (sender, e) =>
            {
                if (sender is Button btn && btn.Tag is Color[] colors)
                {
                    btn.BackColor = colors[1];
                    btn.ForeColor = Color.White;
                    btn.Invalidate();
                }
            };

            button.MouseLeave += (sender, e) =>
            {
                if (sender is Button btn && btn.Tag is Color[] colors)
                {
                    btn.BackColor = colors[0];
                    btn.ForeColor = colors[2];
                    btn.Invalidate();
                }
            };

            return button;
        }

        private static Color Darken(Color color, int amount)
        {
            return UiHelpers.DarkenColor(color, amount);
        }

        private void BtnProducts_Click(object sender, EventArgs e)
        {
            var form = new ProductsForm();
            form.ShowDialog();
        }

        private void BtnIncoming_Click(object sender, EventArgs e)
        {
            var form = new IncomingForm();
            form.ShowDialog();
        }

        private void BtnAdjustment_Click(object sender, EventArgs e)
        {
            var form = new InventoryAdjustmentForm();
            form.ShowDialog();
        }

        private void BtnExpense_Click(object sender, EventArgs e)
        {
            var form = new ExpenseForm();
            form.ShowDialog();
        }

        private void BtnReturns_Click(object sender, EventArgs e)
        {
            var form = new ReturnsForm();
            form.ShowDialog();
        }

        private void BtnStatistics_Click(object sender, EventArgs e)
        {
            var form = new StatisticsForm();
            form.ShowDialog();
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            var form = new OperationHistoryForm();
            form.ShowDialog();
        }
    }
}
