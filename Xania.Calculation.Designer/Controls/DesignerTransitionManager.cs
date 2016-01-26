using System.Drawing;
using System.Windows.Forms;

namespace Xania.Calculation.Designer.Controls
{
    public class DesignerTransitionManager
    {
        private readonly DesignerControl _designerControl;

        public DesignerTransitionManager(DesignerControl designerControl)
        {
            _designerControl = designerControl;

            _designerControl.MouseDown += _designerControl_MouseDown;
            _designerControl.DragEnter += _designerControl_DragEnter;
            _designerControl.DragDrop += _designerControl_DragDrop;
            _designerControl.Paint += _designerControl_Paint;
        }

        void _designerControl_Paint(object sender, PaintEventArgs e)
        {
        }

        void _designerControl_DragDrop(object sender, DragEventArgs e)
        {
            var dragBranch = e.Data.GetData(typeof (DragBranch));
        }

        private void _designerControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof (DragBranch)))
                e.Effect = e.AllowedEffect;

            _designerControl.Invalidate();
        }

        void _designerControl_MouseDown(object sender, MouseEventArgs e)
        {
            _designerControl.DoDragDrop(new DragBranch(e.Location), DragDropEffects.Link);
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