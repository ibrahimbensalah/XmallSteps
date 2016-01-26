using System;
using System.Linq;
using System.Windows.Forms;

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
            if (Control.MouseButtons == MouseButtons.Left && 
                (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.None))
            {
                var q = from cmp in _userControl.Items
                    let pos = e.Location
                    where Math.Abs(pos.X - cmp.X) < 20 && Math.Abs(pos.Y - cmp.Y) < 20
                    select cmp;

                _userControl.SelectedItems =
                    Control.ModifierKeys == Keys.Control && _userControl.SelectedItems != null
                        ? q.Union(_userControl.SelectedItems).ToArray()
                        : q.ToArray();

                _userControl.Invalidate();
            }
        }
    }
}
