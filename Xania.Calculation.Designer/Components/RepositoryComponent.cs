using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Xania.Calculation.Designer.Controls;

namespace Xania.Calculation.Designer.Components
{
    internal class RepositoryComponent : ITreeComponent
    {
        public RepositoryComponent()
        {
            Layout = new ComponentLayout();
        }

        public ComponentLayout Layout { get; private set; }

        public bool Connect(ITreeComponent fromComponent)
        {
            return false;
        }

        public void Paint(Graphics graphics, Font font, Func<ITreeComponent, bool> isSelected)
        {
            var x = Layout.X - Images.db_10_128.Width / 2;
            var y = Layout.Y - Images.db_10_128.Height / 2;

            graphics.DrawImage(Images.db_10_128, new Point(x, y));
        }

        public void UnConnect(ITreeComponent treeComponent)
        {
        }
    }
}