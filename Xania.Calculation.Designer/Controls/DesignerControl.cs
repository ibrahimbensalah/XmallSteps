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
        private DesignerShortcutManager _shortcutManager;

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
        public IEnumerable<NodeComponent> Nodes { get { return _treeComponents.OfType<NodeComponent>(); } }

        public event EventHandler<ITreeComponent[]> SelectionChanged;
        public DesignerControl()
        {
            InitializeComponent();

            _treeComponents = new ObservableCollection<ITreeComponent>();
            _treeComponents.CollectionChanged += CollectionChanged;

            _selectionManager = new DesignerSelectionManager(this);
            _dragDropManager = new DesignerDragDropManager(this);
            _transitionManager = new DesignerTransitionManager(this);
            _shortcutManager = new DesignerShortcutManager(this);
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
                cmp.Paint(e.Graphics, Font, n => SelectedItems.Contains(n));
            }

            base.OnPaint(e);
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

        public void Delete()
        {
            foreach (var s in SelectedItems)
            {
                foreach (var cmp in _treeComponents)
                {
                    cmp.UnConnect(s);
                }
                _treeComponents.Remove(s);
            }


            SelectedItems = new ITreeComponent[0];
        }
    }
}
