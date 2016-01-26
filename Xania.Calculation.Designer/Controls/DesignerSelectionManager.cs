using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Controls
{
    public class DesignerSelectionManager
    {
        private readonly DesignerControl _userControl;

        public DesignerSelectionManager(DesignerControl userControl)
        {
            _userControl = userControl;
            _userControl.MouseDown += _userControl_MouseDown;
        }

        void _userControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                var q = from cmp in _userControl.Items
                    let pos = e.Location
                    where Math.Abs(pos.X - cmp.X) < 20 && Math.Abs(pos.Y - cmp.Y) < 20
                    select cmp;

                _userControl.SelectedItems =
                    Control.ModifierKeys == Keys.Control
                        ? q.Union(_userControl.SelectedItems).ToArray()
                        : q.ToArray();

                _userControl.Invalidate();
            }
        }
    }

    public class DesignerDragManager
    {
        private readonly DesignerControl _userControl;

        public DesignerDragManager(DesignerControl userControl)
        {
            _userControl = userControl;
            _userControl.MouseDown += _userControl_MouseDown;
        }

        void _userControl_MouseDown(object sender, MouseEventArgs e)
        {
            _userControl.DoDragDrop(new DragItem("move"), DragDropEffects.Move);
        }
    }

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
                            cmp.X -= dx;
                            cmp.Y -= dy;
                        }
                        _userControl.Invalidate();
                    }
                }
            }
        }

        void _userControl_MouseDown(object sender, MouseEventArgs e)
        {
            _userControl.DoDragDrop(new DragSelection(e.Location), DragDropEffects.Move);
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
                        _userControl.Add(new LeafComponent { }, pos);
                        break;
                    case "node":
                        _userControl.Add(new NodeComponent { }, pos);
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
