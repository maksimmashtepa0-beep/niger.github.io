using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WareHouse_Pro
{
    public partial class StatisticsForm : Form
    {
        private DataGridView gridTopProducts;
        private DataGridView gridLowStock;
        private Chart chartMovement;
        private Button btnBack;

        public StatisticsForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form_Paint;
            LoadStatistics();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(8, 15, 26), Color.FromArgb(17, 24, 39), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Статистика";
            this.Size = new Size(1000, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 14, 18);

            chartMovement = new Chart
            {
                Location = new Point(20, 20),
                Size = new Size(930, 220),
                BackColor = Color.FromArgb(18, 17, 15),
                BorderlineColor = Color.FromArgb(245, 196, 94),
                BorderlineWidth = 2,
                BorderlineDashStyle = ChartDashStyle.Solid,
                Palette = ChartColorPalette.Bright,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.Normal
            };
            chartMovement.Region = UiHelpers.CreateRoundedRegion(chartMovement.Width, chartMovement.Height, 18);

            var area = new ChartArea("MainArea")
            {
                BackColor = Color.FromArgb(18, 17, 15),
                BackSecondaryColor = Color.FromArgb(40, 35, 28),
                BackGradientStyle = GradientStyle.TopBottom,
                BorderColor = Color.FromArgb(214, 177, 92),
                BorderWidth = 1,
                ShadowColor = Color.FromArgb(214, 177, 92),
                ShadowOffset = 2,
                AxisX = { Title = "Месяцы", LabelStyle = { Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(244, 210, 143) }, MajorGrid = { LineColor = Color.FromArgb(80, 67, 47) }, LineColor = Color.FromArgb(214, 177, 92) },
                AxisY = { Title = "Движение", LabelStyle = { Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(244, 210, 143) }, MajorGrid = { LineColor = Color.FromArgb(80, 67, 47) }, LineColor = Color.FromArgb(214, 177, 92) }
            };
            chartMovement.ChartAreas.Add(area);

            var series = new Series("Movement")
            {
                ChartType = SeriesChartType.SplineArea,
                Color = Color.FromArgb(214, 177, 92),
                BorderWidth = 3,
                BorderColor = Color.FromArgb(164, 121, 48),
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                MarkerColor = Color.FromArgb(244, 210, 143),
                ToolTip = "#VALX: #VAL",
                BackSecondaryColor = Color.FromArgb(167, 123, 59),
                BackGradientStyle = GradientStyle.LeftRight,
                ShadowOffset = 2,
                IsVisibleInLegend = true
            };
            chartMovement.Series.Add(series);

            var legend = new Legend("Legend1")
            {
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(250, 244, 230),
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chartMovement.Legends.Add(legend);

            var title = new Title("Движение товаров по месяцам", Docking.Top, new Font("Segoe UI", 12, FontStyle.Bold), Color.FromArgb(244, 210, 143));
            chartMovement.Titles.Add(title);

            Label lblTop = new Label
            {
                Text = "Топ-5 товаров по движению:",
                Location = new Point(20, 260),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            gridTopProducts = new DataGridView
            {
                Location = new Point(20, 285),
                Size = new Size(930, 150),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(18, 17, 15),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(250, 244, 230),
                GridColor = Color.FromArgb(80, 67, 47),
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(27, 24, 21), ForeColor = Color.FromArgb(250, 244, 230) },
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(214, 177, 92), ForeColor = Color.FromArgb(30, 20, 10), Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                DefaultCellStyle = { BackColor = Color.FromArgb(18, 17, 15), ForeColor = Color.FromArgb(250, 244, 230), SelectionBackColor = Color.FromArgb(151, 122, 67), SelectionForeColor = Color.FromArgb(30, 20, 10) }
            };

            Label lblLow = new Label
            {
                Text = "Товары с низким остатком:",
                Location = new Point(20, 455),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            gridLowStock = new DataGridView
            {
                Location = new Point(20, 480),
                Size = new Size(930, 100),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(18, 17, 15),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(250, 244, 230),
                GridColor = Color.FromArgb(80, 67, 47),
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(27, 24, 21), ForeColor = Color.FromArgb(250, 244, 230) },
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(172, 54, 54), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                DefaultCellStyle = { BackColor = Color.FromArgb(18, 17, 15), ForeColor = Color.FromArgb(250, 244, 230), SelectionBackColor = Color.FromArgb(120, 28, 28), SelectionForeColor = Color.White }
            };

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(20, 590),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(70, 66, 61),
                ForeColor = Color.FromArgb(250, 244, 230),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnBack, Color.FromArgb(70, 66, 61), Color.FromArgb(250, 244, 230));
            btnBack.Click += BtnBack_Click;
            UiHelpers.StyleDataGridView(gridTopProducts, Color.FromArgb(214, 177, 92), Color.FromArgb(91, 77, 48));
            UiHelpers.StyleDataGridView(gridLowStock, Color.FromArgb(172, 54, 54), Color.FromArgb(120, 28, 28));

            this.Controls.Add(chartMovement);
            this.Controls.Add(lblTop);
            this.Controls.Add(gridTopProducts);
            this.Controls.Add(lblLow);
            this.Controls.Add(gridLowStock);
            this.Controls.Add(btnBack);
        }

        private void ApplyButtonStyle(Button button, Color baseColor, Color textColor)
        {
            var hoverColor = DarkenColor(baseColor, 18);
            button.Tag = new[] { baseColor, hoverColor, textColor };
            button.BackColor = baseColor;
            button.ForeColor = textColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            button.Region = CreateRoundedRegion(button.Width, button.Height, 10);
            button.Paint += (sender, e) =>
            {
                var btn = (Button)sender;
                var colors = (Color[])btn.Tag;
                using (var brush = new LinearGradientBrush(btn.ClientRectangle, colors[0], DarkenColor(colors[0], 12), LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRoundedRectangle(brush, 0, 0, btn.Width, btn.Height, 10);
                }

                using (var pen = new Pen(Color.FromArgb(55, 255, 255, 255), 1f))
                {
                    e.Graphics.DrawRoundedRectangle(pen, 1, 1, btn.Width - 2, btn.Height - 2, 10);
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
        }

        private static Region CreateRoundedRegion(int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(width - radius, 0, radius, radius, 270, 90);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddArc(0, height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }

        private static Color DarkenColor(Color color, int amount)
        {
            return Color.FromArgb(
                Math.Max(0, color.R - amount),
                Math.Max(0, color.G - amount),
                Math.Max(0, color.B - amount));
        }

        private void LoadStatistics()
        {
            var chartData = DatabaseHelper.GetMonthlyMovementData();
            chartMovement.Series["Movement"].Points.Clear();

            foreach (DataRow row in chartData.Rows)
            {
                string month = row["MonthName"].ToString();
                int value = Convert.ToInt32(row["NetMovement"]);
                chartMovement.Series["Movement"].Points.AddXY(month, value);
            }

            gridTopProducts.DataSource = DatabaseHelper.GetTopProducts();
            gridLowStock.DataSource = DatabaseHelper.GetLowStockProducts();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
