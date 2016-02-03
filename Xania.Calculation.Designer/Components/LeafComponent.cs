using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xania.Calculation.Designer.Components
{
    public class LeafComponent : TreeComponent
    {
        public LeafComponent()
        {
            Type = LeafType.Amount;
        }

        public IEnumerable<Formula> Formulas { get { return Arguments.OfType<Formula>(); } }

        [Category("Functional"), DisplayName(@"Function")]
        public string Fun { get; set; }

        [Category("Functional"), DisplayName(@"Type")]
        public LeafType Type { get; set; }

        public override bool Connect(ITreeComponent fromComponent)
        {
            if (Arguments.Any(arg => arg.Tree == fromComponent))
                return false;

            if (fromComponent is LeafComponent)
            {
                var name = DesignerHelper.GetNewVariableName("arg{0}", Formulas.Select(a => a.Name));
                Arguments.Add(new Formula { Name = name, Tree = fromComponent });
                return true;
            }
            else if (fromComponent is CsvRepositoryComponent)
            {
                var name = DesignerHelper.GetNewVariableName("arg{0}", Formulas.Select(a => a.Name));
                Arguments.Add(new Repository { Tree = fromComponent });
                return true;
            }

            return false;
        }

        public bool IsFunctionCall
        {
            get
            {
                return Regex.IsMatch(Fun, "^[_a-z][_a-z0-9]+$", RegexOptions.IgnoreCase);
            }
        }

        public bool IsIdentityCall
        {
            get { return string.Equals(Fun, "input"); }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Fun))
                return string.Empty;

            if (IsIdentityCall)
                return Type.ToString();

            if (IsFunctionCall)
                return Fun;
            return string.Format("input -> {0}", Fun);
        }
    }

    public enum LeafType
    {
        Amount,
        Number,
        Text
    }
}