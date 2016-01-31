using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
        }

        private void DesignerControl_DragDrop(object sender, DragEventArgs e)
        {
            var pos = _userControl.PointToClient(new Point(e.X, e.Y));

            var dragItem = e.Data.GetData(typeof(DragItem)) as DragItem;
            if (dragItem != null)
            {
                switch (dragItem.Type.ToLower())
                {
                    case "leaf":
                        _userControl.Add(new LeafComponent {Fun = "input"}, pos);
                        break;
                    case "node":
                        _userControl.Add(
                            new NodeComponent
                            {
                                Name =
                                    DesignerHelper.GetNewVariableName("node{0}", _userControl.Nodes.Select(n => n.Name))
                            }, pos);
                        break;
                    case "connect":
                        _userControl.Add(new ConcatComponent {}, pos);
                        break;
                    case "repository":
                        // _userControl.Add(new CsvRepositoryComponent {}, pos);
                        break;
                }
            }
            else
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                if (files == null) return;

                foreach (var fi in files.Select(file => new FileInfo(file)))
                {
                    _userControl.Add(new CsvRepositoryComponent(fi), pos);
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


    public class DesignerShortcutManager
    {
        private readonly DesignerControl _userControl;

        public DesignerShortcutManager(DesignerControl userControl)
        {
            _userControl = userControl;
            _userControl.KeyDown += _userControl_KeyDown;
        }

        void _userControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _userControl.Delete();
            }
        }
    }
}