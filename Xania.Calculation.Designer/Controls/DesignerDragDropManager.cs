using System.Drawing;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Controls
{
    public class DesignerDragDropManager
    {
        private readonly DesignerControl _userControl;

        public DesignerDragDropManager(DesignerControl userControl)
        {
            _userControl = userControl;
            _userControl.DragDrop += DesignerControl_DragDrop;
            _userControl.DragEnter += DesignerControl_DragEnter;
            _userControl.MouseDown += _userControl_MouseDown;
            _userControl.DragOver += _userControl_DragOver;
        }

        void _userControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof (DragSelection)))
            {
                e.Effect = DragDropEffects.Move;

                var dragSelection = e.Data.GetData(typeof(DragSelection)) as DragSelection;
                if (dragSelection != null)
                {
                    lock (this)
                    {

                        var target = _userControl.PointToClient(new Point(e.X, e.Y));
                        var dx = dragSelection.Location.X - target.X;
                        var dy = dragSelection.Location.Y - target.Y;
                        dragSelection.Location = target;
                        foreach (var cmp in _userControl.SelectedItems)
                        {
                            cmp.Layout.X -= dx;
                            cmp.Layout.Y -= dy;
                        }
                        _userControl.Invalidate();
                    }
                }
            }
        }

        void _userControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.None || Control.ModifierKeys == Keys.Control)
            {
                _userControl.DoDragDrop(new DragSelection(e.Location), DragDropEffects.Move);
            }
        }


        private void DesignerControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragItem)))
                e.Effect = DragDropEffects.Copy;
        }

        private void DesignerControl_DragDrop(object sender, DragEventArgs e)
        {
            var dragItem = e.Data.GetData(typeof(DragItem)) as DragItem;
            if (dragItem != null)
            {
                var pos = _userControl.PointToClient(new Point(e.X, e.Y));
                switch (dragItem.Type.ToLower())
                {
                    case "leaf":
                        _userControl.Add(new LeafComponent { Fun = "value" }, pos);
                        break;
                    case "node":
                        _userControl.Add(new NodeComponent { InputType = "MyObject" }, pos);
                        break;
                }
            }
        }

        internal class DragSelection
        {
            public Point Location { get; set; }

            public DragSelection(Point location)
            {
                Location = location;
            }
        }
    }
}