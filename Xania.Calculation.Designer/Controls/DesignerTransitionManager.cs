using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xania.Calculation.Designer.Controls
{
    public class DesignerTransitionManager
    {
        private readonly DesignerControl _designerControl;
        private Point? _startPos;
        private Point? _endPos;

        public DesignerTransitionManager(DesignerControl designerControl)
        {
            _designerControl = designerControl;

            _designerControl.MouseDown += _designerControl_MouseDown;
            _designerControl.MouseUp += _designerControl_MouseUp;
            _designerControl.Paint += _designerControl_Paint;
            _designerControl.DragEnter += _designerControl_DragEnter;
            _designerControl.DragOver += _designerControl_DragOver;
            _designerControl.DragDrop += _designerControl_DragDrop;
        }

        void _designerControl_MouseUp(object sender, MouseEventArgs e)
        {
            _startPos = null;
            _endPos = null;
        }

        void _designerControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Alt)
            {
                _designerControl.DoDragDrop(new DragBranch(e.Location), DragDropEffects.Link);
            }
        }

        void _designerControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragBranch)))
            {
                _endPos = _designerControl.PointToClient(new Point(e.X, e.Y));
                if (_startPos == null)
                    _startPos = _endPos;
                else
                    _designerControl.Invalidate();
            }
        }

        void _designerControl_Paint(object sender, PaintEventArgs e)
        {
            if (_startPos != null && _endPos != null)
            {
                e.Graphics.DrawLine(Pens.Gray, _startPos.Value, _endPos.Value);
            }
        }

        void _designerControl_DragDrop(object sender, DragEventArgs e)
        {
            _endPos = null;
            _startPos = null;
            _designerControl.Invalidate();
            // var dragBranch = e.Data.GetData(typeof (DragBranch));
        }

        private void _designerControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragBranch)))
                e.Effect = e.AllowedEffect;
        }

        class DragBranch
        {
            private readonly Point _startPoint;

            public DragBranch(Point startPoint)
            {
                _startPoint = startPoint;
            }
        }
    }
}