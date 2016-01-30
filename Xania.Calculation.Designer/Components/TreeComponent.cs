using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Xania.Calculation.Designer.Controls;

namespace Xania.Calculation.Designer.Components
{
    public abstract class TreeComponent: ITreeComponent
    {
        protected TreeComponent()
        {
            Layout = new ComponentLayout();
            Arguments = new ExpandableDesignerCollection<TreeArgument>();
        }

        [Category("Functional")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual ExpandableDesignerCollection<TreeArgument> Arguments { get; private set; }

        public abstract bool Connect(ITreeComponent fromComponent);

        public void Paint(Graphics graphics, Font font, Func<ITreeComponent, bool> isSelected)
        {
            DrawBranches(graphics, font, isSelected);

            var text = ToString();
            var bounds = this.GetBounds(graphics, font);

            var radius = this is LeafComponent ? 10f : 0f;

            DrawStringBounds(graphics, Layout.BackColor, isSelected(this), bounds, radius);
            DrawString(graphics, font, Color.Black, text, Layout.X, Layout.Y);
        }

        public void UnConnect(ITreeComponent treeComponent)
        {
            foreach (var b in Arguments.ToArray())
            {
                if (b.Tree == treeComponent)
                {
                    Arguments.Remove(b);
                }
            }
        }

        private void DrawBranches(Graphics g, Font font, Func<ITreeComponent, bool> isSelected)
        {
            foreach (var b in Arguments)
            {
                var color = isSelected(b.Tree) || isSelected(this)
                    ? Color.Black
                    : Color.Gainsboro;
                g.DrawLine(new Pen(color), Layout.X, Layout.Y, b.Tree.Layout.X, b.Tree.Layout.Y);
                var x = (Layout.X + b.Tree.Layout.X) / 2;
                var y = (Layout.Y + b.Tree.Layout.Y) / 2;

                DrawPointer(g, b.Tree.Layout.X, b.Tree.Layout.Y, Layout.X, Layout.Y);
                DrawString(g, font, color, b.Name, x, y);
            }
        }

        private void DrawPointer(Graphics g, double aX, double aY, double bX, double bY)
        {
            var pos = GetPointerPosition(aX, aY, bX, bY);

            var x = pos.X - Images.arrow_pointer.Width / 2 + 1;
            var y = pos.Y - Images.arrow_pointer.Height / 2 + 1;

            if (Math.Abs(bX - aX) > 0)
            {
                g.TranslateTransform(-pos.X, -pos.Y);
                var rad = (float)Math.Atan((bY - aY) / (bX - aX));
                var deg = rad * 180 / Math.PI + (bX - aX < 0 ? 180 : 0);
                g.RotateTransform((float)deg, MatrixOrder.Append);
                g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
                g.DrawImage(Images.arrow_pointer, x, y);
                g.ResetTransform();
            }
            else
            {
                g.DrawImage(Images.arrow_pointer, x, y);
            }
        }

        private static Point GetPointerPosition(double aX, double aY, double bX, double bY)
        {
            double dX = bX - aX;
            double dY = bY - aY;
            var dZ = Math.Sqrt(dX * dX + dY * dY);
            var f = (dZ - 30) / dZ;
            dX *= f;
            dY *= f;

            return new Point((int)(aX + dX), (int)(aY + dY));
        }

        private void DrawStringBounds(Graphics g, Color backColor, bool selected, RectangleF bounds, float radius)
        {
            g.FillRoundRectangle(new SolidBrush(backColor), bounds.X, bounds.Y, bounds.Width, bounds.Height, radius);
            g.DrawRoundRectangle(Pens.Black, bounds.X, bounds.Y, bounds.Width, bounds.Height, radius);

            if (selected)
            {
                g.DrawRectangle(new Pen(Color.Green) { DashStyle = DashStyle.Dash },
                    bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6);
            }
        }

        private void DrawString(Graphics g, Font font, Color color, string text, int x, int y)
        {
            var textSizeF = g.MeasureString(text, font);
            var textLocationX = x - textSizeF.Width / 2;
            var textLocationY = y - textSizeF.Height / 2;

            g.FillRectangle(Brushes.White, textLocationX - 2, textLocationY - 2, textSizeF.Width + 4, textSizeF.Height + 4);
            g.DrawString(text, font, new SolidBrush(color), textLocationX, textLocationY);
        }

        [Category("Designer")]
        [TypeConverter(typeof(ComponentConverter))]
        public ComponentLayout Layout { get; private set; }
    }
}