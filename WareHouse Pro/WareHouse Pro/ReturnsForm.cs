using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public partial class ReturnsForm : Form
    {
        private ComboBox comboProducts;
        private ComboBox comboReturnType;
        private NumericUpDown nudQuantity;
        private Button btnSave;
        private Button btnBack;

        public ReturnsForm()
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
            this.Text = "Возвраты товара";
            this.Size = new Size(520, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 14, 18);

            Label lblProduct = CreateLabel("Товар:", 30, 35, 100);
            comboProducts = CreateCombo(150, 30, 310, 32);

            Label lblType = CreateLabel("Направление:", 30, 85, 110);
            comboReturnType = CreateCombo(150, 80, 310, 32);
            comboReturnType.Items.AddRange(new object[] { "Возврат от клиента", "Возврат поставщику" });
            comboReturnType.SelectedIndex = 0;

            Label lblQuantity = CreateLabel("Количество:", 30, 135, 100);
            nudQuantity = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 100000,
                Value = 1,
                Location = new Point(150, 130),
                Size = new Size(150, 28),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230)
            };
            nudQuantity.Region = UiHelpers.CreateRoundedRegion(nudQuantity.Width, nudQuantity.Height, 10);

            btnSave = CreateButton("Сохранить", new Point(150, 200), new Size(140, 35), Color.FromArgb(202, 132, 55));
            btnSave.Click += BtnSave_Click;

            btnBack = CreateButton("Назад", new Point(300, 200), new Size(160, 35), Color.FromArgb(70, 66, 61));
            btnBack.Click += BtnBack_Click;

            this.Controls.Add(lblProduct);
            this.Controls.Add(comboProducts);
            this.Controls.Add(lblType);
            this.Controls.Add(comboReturnType);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(nudQuantity);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnBack);
        }

        private static Label CreateLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };
        }

        private static ComboBox CreateCombo(int x, int y, int width, int height)
        {
            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                FlatStyle = FlatStyle.Flat
            };
            combo.Region = UiHelpers.CreateRoundedRegion(combo.Width, combo.Height, 10);
            return combo;
        }

        private static Button CreateButton(string text, Point location, Size size, Color color)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            button.Tag = new[] { color, UiHelpers.DarkenColor(color, 18), Color.White };
            button.Region = UiHelpers.CreateRoundedRegion(button.Width, button.Height, 18);
            button.Paint += (sender, e) =>
            {
                var current = (Button)sender;
                var colors = (Color[])current.Tag;
                using (var brush = new LinearGradientBrush(current.ClientRectangle, colors[0], UiHelpers.DarkenColor(colors[0], 12), LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRoundedRectangle(brush, 0, 0, current.Width, current.Height, 18);
                }

                TextRenderer.DrawText(e.Graphics, current.Text, current.Font, current.ClientRectangle, current.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            };
            button.MouseEnter += (sender, e) =>
            {
                var current = (Button)sender;
                current.BackColor = ((Color[])current.Tag)[1];
                current.Invalidate();
            };
            button.MouseLeave += (sender, e) =>
            {
                var current = (Button)sender;
                current.BackColor = ((Color[])current.Tag)[0];
                current.Invalidate();
            };
            return button;
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
            if (comboProducts.SelectedValue == null || comboReturnType.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар и направление возврата.", "Внимание");
                return;
            }

            try
            {
                DatabaseHelper.AddOperation(
                    Convert.ToInt32(comboProducts.SelectedValue),
                    comboReturnType.SelectedItem.ToString(),
                    (int)nudQuantity.Value);

                MessageBox.Show("Возврат сохранён.", "Успех");
                nudQuantity.Value = 1;
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