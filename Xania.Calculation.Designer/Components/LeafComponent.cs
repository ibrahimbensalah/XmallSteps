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
            Layout = new ComponentLayout();
        }

        [Category("Functional"), DisplayName(@"Input type")]
        public string InputType { get; set; }

        public ComponentLayout Layout { get; private set; }

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