using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xania.Calculation.Designer.Controls
{
    public class TreeBranchesManager
    {
        private Point _mouseDownPosition;

        public TreeBranchesManager(UserControl userControl)
        {
            var userControl1 = userControl;

            userControl1.MouseDown += _userControl_MouseDown;
            userControl1.MouseMove += _userControl_MouseMove;
        }

        void _userControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {

            }
        }

        void _userControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                _mouseDownPosition = e.Location;
            }
        }
    }

    public class DesignerSelectionManager
    {
        public static void Attach(UserControl userControl)
        {
            // userControl.MouseD
        }
    }
}
