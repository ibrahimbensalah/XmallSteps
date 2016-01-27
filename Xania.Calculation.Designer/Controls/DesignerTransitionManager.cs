using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

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
            _designerControl.DragOver += _designerControl_DragOver;
            _designerControl.DragDrop += _designerControl_DragDrop;
        }

        void _designerControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Alt)
            {
                var dragBranch = new DragBranch(e.Location);
                _designerControl.DoDragDrop(dragBranch, DragDropEffects.Link);
            }
        }

        void _designerControl_DragOver(object sender, DragEventArgs eventArgs)
        {
            if (eventArgs.Data.GetDataPresent(typeof(DragBranch)))
            {
                var dragBranch = (DragBranch) eventArgs.Data.GetData(typeof (DragBranch));
                var toLocation = _designerControl.PointToClient(new Point(eventArgs.X, eventArgs.Y));
                _designerControl.DoPaint(g =>
                {
                    var fromComponent =
                         _designerControl.FindComponents(dragBranch.FromLocation)
                             .FirstOrDefault();

                    if (fromComponent != null)
                        g.DrawLine(Pens.Gray, fromComponent.Layout.X, fromComponent.Layout.Y, toLocation.X, toLocation.Y);
                });
            }
        }

        void _designerControl_DragDrop(object sender, DragEventArgs eventArgs)
        {
            if (eventArgs.Data.GetDataPresent(typeof (DragBranch)))
            {
                var dragBranch = (DragBranch) eventArgs.Data.GetData(typeof (DragBranch));
                var endLocation = _designerControl.PointToClient(new Point(eventArgs.X, eventArgs.Y));

                var fromComponent = _designerControl.FindComponents(dragBranch.FromLocation).FirstOrDefault();
                var endComponent = _designerControl.FindComponents(endLocation).OfType<NodeComponent>().FirstOrDefault();

                if (endComponent != null && fromComponent != null && fromComponent != endComponent)
                {
                    endComponent.Branches.Add(new BranchComponent {Name = "branch", Tree = fromComponent});
                }

                _designerControl.Invalidate();
            }
            // var dragBranch = e.Data.GetData(typeof (DragBranch));
        }

        private void _designerControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragBranch)))
                e.Effect = e.AllowedEffect;
        }

        class DragBranch
        {
            public Point FromLocation { get; private set; }

            public DragBranch(Point fromLocation)
            {
                FromLocation = fromLocation;
            }
        }
    }
}