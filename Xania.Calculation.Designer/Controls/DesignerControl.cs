using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Controls
{
    public partial class DesignerControl : UserControl
    {
        private readonly ObservableCollection<ITreeComponent> _treeComponents;
        private DesignerSelectionManager _selectionManager;
        private ITreeComponent[] _selectedItems;
        private DesignerDragDropManager _dragDropManager;
        private DesignerTransitionManager _transitionManager;
        private readonly IList<Action<Graphics>> _drawActions = new List<Action<Graphics>>();

        public ITreeComponent[] SelectedItems
        {
            get { return _selectedItems ?? new ITreeComponent[0]; }
            set
            {
                if (value != _selectedItems)
                {
                    _selectedItems = value;
                    OnSelectionChanged();
                }
            }
        }

        public IEnumerable<ITreeComponent> Items { get { return _treeComponents; } }

        public event EventHandler<ITreeComponent[]> SelectionChanged;
        public DesignerControl()
        {
            InitializeComponent();

            _treeComponents = new ObservableCollection<ITreeComponent>();
            _treeComponents.CollectionChanged += CollectionChanged;

            _selectionManager = new DesignerSelectionManager(this);
            _dragDropManager = new DesignerDragDropManager(this);
            _transitionManager = new DesignerTransitionManager(this);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            lock (_drawActions)
            {
                foreach (var a in _drawActions.ToArray())
                {
                    a(e.Graphics);
                }
                _drawActions.Clear();
            }

            foreach (var cmp in _treeComponents)
            {
                DrawBranches(cmp, e.Graphics);
            }

            foreach (var cmp in _treeComponents)
            {
                var text = cmp.ToString();
                var bounds = cmp.GetBounds(e.Graphics, Font);

                DrawStringBounds(e.Graphics, cmp.Layout.BackColor, SelectedItems.Contains(cmp), bounds);
                DrawString(e.Graphics, Color.Black, text, cmp.Layout.X, cmp.Layout.Y);
            }

            base.OnPaint(e);
        }

        private void DrawBranches(ITreeComponent node, Graphics g)
        {
            foreach (var b in node.Arguments)
            {
                var color = SelectedItems.Contains(b.Tree) || SelectedItems.Contains(node)
                    ? Color.Black
                    : Color.Gainsboro;
                g.DrawLine(new Pen(color), node.Layout.X, node.Layout.Y, b.Tree.Layout.X, b.Tree.Layout.Y);
                var x = (node.Layout.X + b.Tree.Layout.X)/2;
                var y = (node.Layout.Y + b.Tree.Layout.Y)/2;

                DrawPointer(g, b.Tree.Layout.X, b.Tree.Layout.Y, node.Layout.X, node.Layout.Y);
                DrawString(g, color, b.Name, x, y);
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
                var rad = (float) Math.Atan((bY - aY)/(bX - aX));
                var deg = rad*180/Math.PI;
                g.RotateTransform((float) deg, MatrixOrder.Append);
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
            var dZ = Math.Sqrt(dX*dX + dY*dY);
            var f = (dZ - 30)/dZ;
            dX *= f;
            dY *= f;

            return new Point((int)(aX + dX), (int)(aY + dY));
        }

        private void DrawStringBounds(Graphics g, Color backColor, bool selected, RectangleF bounds)
        {
            g.FillRectangle(new SolidBrush(backColor), bounds);
            g.DrawRectangle(Pens.Black, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            if (selected)
            {
                g.DrawRectangle(new Pen(Color.Green) { DashStyle = DashStyle.Dash },
                    bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6);
            }
        }

        private void DrawString(Graphics g, Color color, string text, int x, int y)
        {
            var textSizeF = g.MeasureString(text, Font);
            var textLocationX = x - textSizeF.Width / 2;
            var textLocationY = y - textSizeF.Height / 2;

            g.FillRectangle(Brushes.White, textLocationX - 2, textLocationY - 2, textSizeF.Width + 4, textSizeF.Height + 4);
            g.DrawString(text, Font, new SolidBrush(color), textLocationX, textLocationY);
        }

        public void Add(ITreeComponent cmp, Point pos)
        {
            cmp.MoveTo(pos);

            _treeComponents.Add(cmp);
            SelectedItems = new[] { cmp };
        }

        protected virtual void OnSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null) handler(this, SelectedItems);
        }

        public void DoPaint(Action<Graphics> action)
        {
            lock (_drawActions)
            {
                _drawActions.Add(action);
                Invalidate();
            }
        }

        public IEnumerable<ITreeComponent> FindComponents(Point point)
        {
            return Items.Where(i => i.Layout.Bounds.Contains(point));
        }
    }
}
