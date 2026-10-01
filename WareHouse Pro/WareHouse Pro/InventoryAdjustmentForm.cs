using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public partial class InventoryAdjustmentForm : Form
    {
        private ComboBox comboProducts;
        private NumericUpDown nudDelta;
        private TextBox txtReason;
        private Button btnSave;
        private Button btnBack;

        public InventoryAdjustmentForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form_Paint;
            LoadProducts();
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
            this.Text = "Корректировка остатков";
            this.Size = new Size(500, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 14, 18);

            Label lblProduct = new Label
            {
                Text = "Товар:",
                Location = new Point(30, 35),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            comboProducts = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(150, 30),
                Size = new Size(290, 32),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                FlatStyle = FlatStyle.Flat
            };
            comboProducts.Region = UiHelpers.CreateRoundedRegion(comboProducts.Width, comboProducts.Height, 10);

            Label lblDelta = new Label
            {
                Text = "Изменение:",
                Location = new Point(30, 85),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            nudDelta = new NumericUpDown
            {
                Minimum = -100000,
                Maximum = 100000,
                Value = 1,
                Increment = 1,
                Location = new Point(150, 80),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230)
            };
            nudDelta.Region = UiHelpers.CreateRoundedRegion(nudDelta.Width, nudDelta.Height, 10);

            Label lblReason = new Label
            {
                Text = "Причина:",
                Location = new Point(30, 140),
                Size = new Size(90, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            txtReason = new TextBox
            {
                Location = new Point(150, 135),
                Size = new Size(290, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtReason.Region = UiHelpers.CreateRoundedRegion(txtReason.Width, txtReason.Height, 10);

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(150, 200),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(56, 142, 168),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnSave, Color.FromArgb(56, 142, 168), Color.White);
            btnSave.Click += BtnSave_Click;

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(290, 200),
                Size = new Size(150, 35),
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

            this.Controls.Add(lblProduct);
            this.Controls.Add(comboProducts);
            this.Controls.Add(lblDelta);
            this.Controls.Add(nudDelta);
            this.Controls.Add(lblReason);
            this.Controls.Add(txtReason);
            this.Controls.Add(btnSave);
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

        private void LoadProducts()
        {
            DataTable products = DatabaseHelper.GetProductsForCombo();
            comboProducts.DataSource = products;
            comboProducts.DisplayMember = "Name";
            comboProducts.ValueMember = "Id";

            if (comboProducts.Items.Count > 0)
            {
                comboProducts.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (comboProducts.SelectedValue == null)
            {
                MessageBox.Show("Сначала добавьте товар.", "Внимание");
                return;
            }

            try
            {
                int productId = Convert.ToInt32(comboProducts.SelectedValue);
                int delta = (int)nudDelta.Value;

                DatabaseHelper.AdjustStock(productId, delta, txtReason.Text.Trim());
                MessageBox.Show("Корректировка остатков сохранена.", "Успех");
                nudDelta.Value = 1;
                txtReason.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}