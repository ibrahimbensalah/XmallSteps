using System;
using System.Collections.Concurrent;
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
                var text = cmp.ToString();
                var bounds = cmp.GetBounds(e.Graphics, Font);

                DrawStringBounds(cmp, e.Graphics, bounds);
                DrawString(cmp, e.Graphics, text);
            }

            base.OnPaint(e);
        }

        private void DrawStringBounds(ITreeComponent cmp, Graphics g, RectangleF bounds)
        {
            //var textWidth = textSizeF.Width;
            //var textHeight = textSizeF.Height;

            //var textBoundsX = cmp.X - textSizeF.Width/2;
            //var textBoundsY = cmp.Y - textSizeF.Height/2;

            g.FillRectangle(new SolidBrush(cmp.Layout.BackColor), bounds);
            g.DrawRectangle(Pens.Black, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            if (SelectedItems.Contains(cmp))
            {
                g.DrawRectangle(new Pen(Color.Green) {DashStyle = DashStyle.Dash},
                    bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6);
                    // textBoundsX - 3, textBoundsY - 3, textWidth + 6, textHeight + 6);
            }
        }

        private void DrawString(ITreeComponent cmp, Graphics g, string text)
        {
            var textSizeF = g.MeasureString(text, Font);
            var textLocationX = cmp.Layout.X - textSizeF.Width / 2;
            var textLocationY = cmp.Layout.Y - textSizeF.Height / 2;
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

        public void Invalidate(Action<Graphics> action)
        {
            lock (_drawActions)
            {
                _drawActions.Add(action);
                Invalidate();
            }
        }
    }
}
