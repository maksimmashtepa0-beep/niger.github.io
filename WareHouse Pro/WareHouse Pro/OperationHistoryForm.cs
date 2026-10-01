using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public partial class OperationHistoryForm : Form
    {
        private DataGridView gridOperations;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnBack;

        public OperationHistoryForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form_Paint;
            LoadOperations();
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
            this.Text = "История операций";
            this.Size = new Size(980, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 14, 18);

            Label lblSearch = new Label
            {
                Text = "Поиск:",
                Location = new Point(20, 20),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            txtSearch = new TextBox
            {
                Location = new Point(90, 17),
                Size = new Size(260, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.Region = UiHelpers.CreateRoundedRegion(txtSearch.Width, txtSearch.Height, 10);

            btnSearch = new Button
            {
                Text = "Найти",
                Location = new Point(370, 15),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(99, 78, 179),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnSearch, Color.FromArgb(99, 78, 179), Color.White);
            btnSearch.Click += BtnSearch_Click;

            gridOperations = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(930, 430),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(18, 17, 15),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(250, 244, 230),
                GridColor = Color.FromArgb(80, 67, 47),
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(27, 24, 21), ForeColor = Color.FromArgb(250, 244, 230) },
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(99, 78, 179), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                DefaultCellStyle = { BackColor = Color.FromArgb(18, 17, 15), ForeColor = Color.FromArgb(250, 244, 230), SelectionBackColor = Color.FromArgb(120, 99, 255), SelectionForeColor = Color.White }
            };

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(20, 510),
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
            UiHelpers.StyleDataGridView(gridOperations, Color.FromArgb(99, 78, 179), Color.FromArgb(72, 61, 126));

            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(gridOperations);
            this.Controls.Add(btnBack);
        }

        private void ApplyButtonStyle(Button button, Color baseColor, Color textColor)
        {
            var hoverColor = UiHelpers.DarkenColor(baseColor, 18);
            button.Tag = new[] { baseColor, hoverColor, textColor };
            button.BackColor = baseColor;
            button.ForeColor = textColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            button.Region = UiHelpers.CreateRoundedRegion(button.Width, button.Height, 10);
            button.Paint += (sender, e) =>
            {
                var btn = (Button)sender;
                var colors = (Color[])btn.Tag;
                using (var brush = new LinearGradientBrush(btn.ClientRectangle, colors[0], UiHelpers.DarkenColor(colors[0], 12), LinearGradientMode.Vertical))
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

        private void LoadOperations()
        {
            gridOperations.DataSource = DatabaseHelper.GetOperationsTable(txtSearch.Text.Trim());
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadOperations();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
