using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Xania.Calculation.Designer.Components
{
    public class LeafComponent : TreeComponent
    {
        public LeafComponent()
        {
            Type = LeafType.Amount;
        }

        [Category("Functional"), DisplayName(@"Function")]
        public string Fun { get; set; }

        [Category("Functional")]
        [DisplayName(@"Leaf type")]
        public LeafType Type { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Fun))
                return string.Empty;

            if (Regex.IsMatch(Fun, "^[_a-z][_a-z0-9]+$", RegexOptions.IgnoreCase))
                return string.Format("{0} {1}", Type, Fun);
            return string.Format("{0} (fun value -> {1})", Type, Fun);
        }
    }

    public abstract class TreeComponent: ITreeComponent
    {
        protected TreeComponent()
        {
            W = 50;
            H = 50;
        }

        [Category("Functional"), DisplayName(@"Input type")]
        public string InputType { get; set; }

        [Browsable(false)]
        public int X { get; set; }
        [Browsable(false)]
        public int Y { get; set; }
        [Browsable(false)]
        public int W { get; set; }
        [Browsable(false)]
        public int H { get; set; }

        [Category("Layout")]
        public Color BackColor { get; set; }

        [Category("Layout")]
        public Color ForeColor { get; set; }

        public override string ToString()
        {
            return this.InputType;
        }
    }

    public enum LeafType
    {
        Amount,
        Number
    }
}