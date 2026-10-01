using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouse_Pro
{
    public static class UiHelpers
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, int x, int y, int width, int height, int radius)
        {
            using (var path = CreateRoundedRectPath(x, y, width, height, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, int x, int y, int width, int height, int radius)
        {
            using (var path = CreateRoundedRectPath(x, y, width, height, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static Region CreateRoundedRegion(int width, int height, int radius)
        {
            using (var path = CreateRoundedRectPath(0, 0, width, height, radius))
            {
                return new Region(path);
            }
        }

        public static GraphicsPath CreateRoundedRectPath(int x, int y, int width, int height, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(width, height) / 2);

            if (r <= 0)
            {
                path.AddRectangle(new Rectangle(x, y, width, height));
                return path;
            }

            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + width - (r * 2), y, r * 2, r * 2, 270, 90);
            path.AddArc(x + width - (r * 2), y + height - (r * 2), r * 2, r * 2, 0, 90);
            path.AddArc(x, y + height - (r * 2), r * 2, r * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        public static Color DarkenColor(Color color, int amount)
        {
            return Color.FromArgb(
                Math.Max(0, color.R - amount),
                Math.Max(0, color.G - amount),
                Math.Max(0, color.B - amount));
        }

        public static void StyleDataGridView(DataGridView grid, Color headerColor, Color selectionColor)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            grid.DefaultCellStyle.SelectionBackColor = selectionColor;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(27, 31, 40);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(239, 243, 248);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }
    }
}
