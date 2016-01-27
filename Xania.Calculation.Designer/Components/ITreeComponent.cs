using System;
using System.ComponentModel;
using System.Drawing;

namespace Xania.Calculation.Designer.Components
{
    public interface ITreeComponent
    {
        string InputType { get; }

        ComponentLayout Layout { get; }
        ExpandableDesignerCollection<TreeArgument> Arguments { get; }
        bool Connect(ITreeComponent fromComponent);
    }

    public class ComponentLayout
    {
        public ComponentLayout()
        {
           BackColor = Color.White;
        }
        [Browsable(false)]
        public int X { get; set; }
        [Browsable(false)]
        public int Y { get; set; }
        [Browsable(false)]
        public RectangleF Bounds { get; set; }

        public Color BackColor { get; set; }
    }

    public static class TreeComponentExtensions
    {
        public static RectangleF GetBounds(this ITreeComponent treeComponent, Graphics g, Font font)
        {
            var text = treeComponent.ToString();
            var textSizeF = g.MeasureString(text, font);

            var w = Math.Max(40, textSizeF.Width + 10);
            var h = Math.Max(20, textSizeF.Height + 10);
            var x = treeComponent.Layout.X - w / 2;
            var y = treeComponent.Layout.Y - h / 2;

            return treeComponent.Layout.Bounds = new RectangleF(x, y, w, h);
        }

        public static ITreeComponent MoveTo(this ITreeComponent treeComponent, Point pos)
        {
            treeComponent.Layout.X = pos.X;
            treeComponent.Layout.Y = pos.Y;

            return treeComponent;
        }
    }
}