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

                DrawStringBounds(cmp, e.Graphics, bounds);
                DrawString(e.Graphics, text, cmp.Layout.X, cmp.Layout.Y);
            }

            base.OnPaint(e);
        }

        private void DrawBranches(ITreeComponent cmp, Graphics g)
        {
            var node = cmp as NodeComponent;
            if (node != null)
            {
                foreach (var b in node.Branches)
                {
                    var pen = SelectedItems.Contains(b.Tree) || SelectedItems.Contains(node) ? Pens.Gainsboro : Pens.Black;
                    g.DrawLine(pen, node.Layout.X, node.Layout.Y, b.Tree.Layout.X, b.Tree.Layout.Y);
                    
                    {
                        var x = (node.Layout.X + b.Tree.Layout.X) / 2;
                        var y = (node.Layout.Y + b.Tree.Layout.Y) / 2;
                        DrawString(g, b.Name, x, y);
                        // g.DrawString(b.Name + " => " + b.Path, Font, Brushes.Gray, x, y);
                    }
                }
            }
        }

        private void DrawStringBounds(ITreeComponent cmp, Graphics g, RectangleF bounds)
        {
            g.FillRectangle(new SolidBrush(cmp.Layout.BackColor), bounds);
            g.DrawRectangle(Pens.Black, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            if (SelectedItems.Contains(cmp))
            {
                g.DrawRectangle(new Pen(Color.Green) {DashStyle = DashStyle.Dash},
                    bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6);
            }
        }

        private void DrawString(Graphics g, string text, int x, int y)
        {
            var textSizeF = g.MeasureString(text, Font);
            var textLocationX = x - textSizeF.Width / 2;
            var textLocationY = y - textSizeF.Height / 2;
            g.DrawString(text, Font, Brushes.Black, textLocationX, textLocationY);
        }

        public void Add(ITreeComponent cmp, Point pos)
        {
            cmp.MoveTo(pos);

            _treeComponents.Add(cmp);
            SelectedItems = new[] {cmp};
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
