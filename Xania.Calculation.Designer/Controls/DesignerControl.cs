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
        private TreeBranchesManager _treeBranchesManager;
        public ITreeComponent[] SelectedItems { get; private set; }

        public event EventHandler<ITreeComponent[]> SelectionChanged;
        public DesignerControl()
        {
            InitializeComponent();

            _treeComponents = new ObservableCollection<ITreeComponent>();
            _treeComponents.CollectionChanged += CollectionChanged;

            _treeBranchesManager = new TreeBranchesManager(this);
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
                var text = cmp.ToString();
                var textSizeF = e.Graphics.MeasureString(text, Font);

                DrawStringBounds(e.Graphics, textSizeF, cmp);
                DrawString(e.Graphics, cmp.X, cmp.Y, textSizeF, text);
            }
        }

        private static void DrawStringBounds(Graphics g, SizeF textSizeF, ITreeComponent cmp)
        {
            var textWidth = Math.Max(40, textSizeF.Width + 10);
            var textHeight = Math.Max(20, textSizeF.Height + 10);
            var textBoundsX = cmp.X - textWidth / 2;
            var textBoundsY = cmp.Y - textHeight/2;
            g.FillRectangle(new SolidBrush(cmp.BackColor), textBoundsX, textBoundsY, textWidth, textHeight);
            g.DrawRectangle(Pens.Black, textBoundsX, textBoundsY, textWidth, textHeight);
        }

        private void DrawString(Graphics g, int x, int y, SizeF textSizeF, string text)
        {
            var textLocationX = x - textSizeF.Width/2;
            var textLocationY = y - textSizeF.Height/2;
            g.DrawString(text, Font, Brushes.Black, textLocationX, textLocationY);
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
