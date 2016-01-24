using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Xania.Calculation.Designer.Components
{
    public interface ITreeComponent
    {
        string InputType { get; }

        int X { get; set; }
        int Y { get; set; }
        int W { get; }
        int H { get; }
    }

    public static class TreeComponentExtensions
    {
        public static Rectangle GetBounds(this ITreeComponent treeComponent)
        {
            return new Rectangle(treeComponent.X, treeComponent.Y, treeComponent.W, treeComponent.H);
        }

        public static ITreeComponent MoveTo(this ITreeComponent treeComponent, Point pos)
        {
            treeComponent.X = pos.X - treeComponent.W/2;
            treeComponent.Y = pos.Y - treeComponent.H/2;

            return treeComponent;
        }
    }
}