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
            if (Control.MouseButtons == MouseButtons.Left && 
                (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.None))
            {
                _userControl.Invalidate(g =>
                {
                    var q = from cmp in _userControl.Items
                            // let pos = e.Location
                            let bounds = cmp.GetBounds(g, _userControl.Font)
                            where bounds.Contains(e.Location)
                            // where Math.Abs(bounds.X - cmp.X) < 20 && Math.Abs(bounds.Y - cmp.Y) < 20
                            select cmp;

                    _userControl.SelectedItems =
                        Control.ModifierKeys == Keys.Control && _userControl.SelectedItems != null
                            ? q.Union(_userControl.SelectedItems).ToArray()
                            : q.ToArray();
                });
            }
        }
    }
}
