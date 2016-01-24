using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Controls
{
    public partial class DesignerControl : UserControl
    {
        private readonly ObservableCollection<ITreeComponent> _treeComponents;
        public ITreeComponent[] SelectedItems { get; private set; }

        public event EventHandler<ITreeComponent[]> SelectionChanged;
        public DesignerControl()
        {
            InitializeComponent();

            _treeComponents = new ObservableCollection<ITreeComponent>();
            _treeComponents.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var cmp in _treeComponents)
            {
                e.Graphics.DrawRectangle(Pens.Black, cmp.GetBounds());
                e.Graphics.DrawString(cmp.ToString(), this.Font, Brushes.Black, cmp.GetBounds());
            }
        }

        private void DesignerControl_DragDrop(object sender, DragEventArgs e)
        {
            var dragItem = e.Data.GetData(typeof(DragItem)) as DragItem;
            if (dragItem != null)
            {
                var pos = PointToClient(new Point(e.X, e.Y));
                var tree = dragItem.Tree.MoveTo(pos);
                _treeComponents.Add(tree);

                SelectedItems = new [] {tree};
                OnSelectionChanged();
            }
        }

        private void DesignerControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragItem)))
                e.Effect = DragDropEffects.Copy;
        }

        protected virtual void OnSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null) handler(this, SelectedItems);
        }
    }
}
