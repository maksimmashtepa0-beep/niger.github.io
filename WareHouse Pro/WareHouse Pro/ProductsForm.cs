using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public partial class ProductsForm : Form
    {
        private DataGridView dataGridViewProducts;
        private TextBox txtSearch;
        private TextBox txtName;
        private TextBox txtArticle;
        private TextBox txtCategory;
        private NumericUpDown nudQuantity;
        private NumericUpDown nudMinStock;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private int selectedProductId = -1;

        public ProductsForm()
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
            this.Text = "Товары";
            this.Size = new Size(920, 570);
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
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(214, 177, 92),
                ForeColor = Color.FromArgb(30, 20, 10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnSearch, Color.FromArgb(214, 177, 92), Color.FromArgb(30, 20, 10));
            btnSearch.Click += BtnSearch_Click;

            dataGridViewProducts = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(860, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                BackgroundColor = Color.FromArgb(18, 17, 15),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(250, 244, 230),
                GridColor = Color.FromArgb(80, 67, 47),
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(27, 24, 21), ForeColor = Color.FromArgb(250, 244, 230) },
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(245, 196, 94), ForeColor = Color.FromArgb(30, 20, 10), Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                DefaultCellStyle = { BackColor = Color.FromArgb(18, 17, 15), ForeColor = Color.FromArgb(250, 244, 230), SelectionBackColor = Color.FromArgb(151, 122, 67), SelectionForeColor = Color.FromArgb(30, 20, 10) }
            };

            Label lblName = new Label
            {
                Text = "Название:",
                Location = new Point(20, 335),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            txtName = new TextBox
            {
                Location = new Point(110, 332),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtName.Region = UiHelpers.CreateRoundedRegion(txtName.Width, txtName.Height, 10);

            Label lblArticle = new Label
            {
                Text = "Артикул:",
                Location = new Point(330, 335),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            txtArticle = new TextBox
            {
                Location = new Point(405, 332),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtArticle.Region = UiHelpers.CreateRoundedRegion(txtArticle.Width, txtArticle.Height, 10);

            Label lblCategory = new Label
            {
                Text = "Категория:",
                Location = new Point(535, 335),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            txtCategory = new TextBox
            {
                Location = new Point(620, 332),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtCategory.Region = UiHelpers.CreateRoundedRegion(txtCategory.Width, txtCategory.Height, 10);

            Label lblQuantity = new Label
            {
                Text = "Кол-во:",
                Location = new Point(20, 380),
                Size = new Size(75, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            nudQuantity = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                Location = new Point(110, 377),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230)
            };
            nudQuantity.Region = UiHelpers.CreateRoundedRegion(nudQuantity.Width, nudQuantity.Height, 10);

            Label lblMinStock = new Label
            {
                Text = "Мин. ост:",
                Location = new Point(270, 380),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 210, 143)
            };

            nudMinStock = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                Value = 0,
                Location = new Point(360, 377),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(26, 32, 41),
                ForeColor = Color.FromArgb(250, 244, 230)
            };
            nudMinStock.Region = UiHelpers.CreateRoundedRegion(nudMinStock.Width, nudMinStock.Height, 10);

            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(760, 325),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(25, 150, 115),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnAdd, Color.FromArgb(25, 150, 115), Color.White);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Изменить",
                Location = new Point(760, 370),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(99, 78, 179),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnEdit, Color.FromArgb(99, 78, 179), Color.White);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(760, 415),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(172, 54, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ApplyButtonStyle(btnDelete, Color.FromArgb(172, 54, 54), Color.White);
            btnDelete.Click += BtnDelete_Click;

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(20, 485),
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

            dataGridViewProducts.SelectionChanged += DataGridViewProducts_SelectionChanged;
            UiHelpers.StyleDataGridView(dataGridViewProducts, Color.FromArgb(214, 177, 92), Color.FromArgb(91, 77, 48));

            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dataGridViewProducts);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblArticle);
            this.Controls.Add(txtArticle);
            this.Controls.Add(lblCategory);
            this.Controls.Add(txtCategory);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(nudQuantity);
            this.Controls.Add(lblMinStock);
            this.Controls.Add(nudMinStock);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
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

        private void DataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count == 0)
            {
                selectedProductId = -1;
                return;
            }

            var row = dataGridViewProducts.SelectedRows[0];
            selectedProductId = Convert.ToInt32(row.Cells["Id"].Value);
            txtName.Text = row.Cells["Name"].Value?.ToString();
            txtArticle.Text = row.Cells["Article"].Value?.ToString();
            txtCategory.Text = row.Cells["Category"].Value?.ToString();
            nudQuantity.Value = Convert.ToInt32(row.Cells["Quantity"].Value);
            nudMinStock.Value = row.Cells["MinimumStock"].Value == DBNull.Value ? 0 : Convert.ToInt32(row.Cells["MinimumStock"].Value);
        }

        private void LoadProducts()
        {
            dataGridViewProducts.DataSource = DatabaseHelper.GetProductsTable(txtSearch.Text.Trim());
            selectedProductId = -1;
            txtName.Clear();
            txtArticle.Clear();
            txtCategory.Clear();
            nudQuantity.Value = 0;
            nudMinStock.Value = 0;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string article = txtArticle.Text.Trim();
            string category = txtCategory.Text.Trim();
            int quantity = (int)nudQuantity.Value;
            int minimumStock = (int)nudMinStock.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название товара.", "Внимание");
                return;
            }

            try
            {
                DatabaseHelper.AddProduct(name, quantity, article, category, minimumStock);
                MessageBox.Show("Товар добавлен.", "Успех");
                txtName.Clear();
                txtArticle.Clear();
                txtCategory.Clear();
                nudQuantity.Value = 0;
                nudMinStock.Value = 0;
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (selectedProductId <= 0)
            {
                MessageBox.Show("Сначала выберите товар из списка.", "Внимание");
                return;
            }

            string name = txtName.Text.Trim();
            string article = txtArticle.Text.Trim();
            string category = txtCategory.Text.Trim();
            int quantity = (int)nudQuantity.Value;
            int minimumStock = (int)nudMinStock.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название товара.", "Внимание");
                return;
            }

            try
            {
                DatabaseHelper.UpdateProduct(selectedProductId, name, quantity, article, category, minimumStock);
                MessageBox.Show("Изменения сохранены.", "Успех");
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для удаления.", "Внимание");
                return;
            }

            int productId = Convert.ToInt32(dataGridViewProducts.SelectedRows[0].Cells["Id"].Value);

            var result = MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteProduct(productId);
                    MessageBox.Show("Товар удалён.", "Успех");
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
