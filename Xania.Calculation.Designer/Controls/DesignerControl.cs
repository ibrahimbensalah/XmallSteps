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

        public ITreeComponent[] SelectedItems
        {
            get { return _selectedItems; }
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
            foreach (var cmp in _treeComponents)
            {
                var text = cmp.ToString();
                var textSizeF = e.Graphics.MeasureString(text, Font);

                DrawStringBounds(cmp, e.Graphics, textSizeF);
                DrawString(cmp, e.Graphics, textSizeF, text);
            }

            base.OnPaint(e);
        }

        private void DrawStringBounds(ITreeComponent cmp, Graphics g, SizeF textSizeF)
        {
            var textWidth = Math.Max(40, textSizeF.Width + 10);
            var textHeight = Math.Max(20, textSizeF.Height + 10);
            var textBoundsX = cmp.X - textWidth/2;
            var textBoundsY = cmp.Y - textHeight/2;
            g.FillRectangle(new SolidBrush(cmp.BackColor), textBoundsX, textBoundsY, textWidth, textHeight);
            g.DrawRectangle(Pens.Black, textBoundsX, textBoundsY, textWidth, textHeight);

            if (SelectedItems.Contains(cmp))
            {
                g.DrawRectangle(new Pen(Color.Green) { DashStyle = DashStyle.Dash }, 
                    textBoundsX - 3, textBoundsY - 3, textWidth + 6, textHeight + 6);
            }
        }

        private void DrawString(ITreeComponent cmp, Graphics g, SizeF textSizeF, string text)
        {
            var textLocationX = cmp.X - textSizeF.Width/2;
            var textLocationY = cmp.Y - textSizeF.Height / 2;
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
    }
}
