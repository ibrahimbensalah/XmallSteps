using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xania.Calculation.Designer.Components
{
    public class LeafComponent : TreeComponent
    {
        [Category("Functional"), DisplayName(@"Function")]
        public string Fun { get; set; }

        public override bool Connect(ITreeComponent fromComponent)
        {
            if (!(fromComponent is LeafComponent) || Arguments.Any(arg => arg.Tree == fromComponent))
                return false;

            Arguments.Add(new TreeArgument { Name = "arg0", Tree = fromComponent });
            return true;
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

            if (IsFunctionCall || IsIdentityCall)
                return Fun;
            return string.Format("input -> {0}", Fun);
        }
    }

    public abstract class TreeComponent: ITreeComponent
    {
        protected TreeComponent()
        {
            Layout = new ComponentLayout();
            Arguments = new ExpandableDesignerCollection<TreeArgument>();
        }

        [Category("Functional"), DisplayName(@"Input type")]
        public string InputType { get; set; }

        [Category("Functional")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual ExpandableDesignerCollection<TreeArgument> Arguments { get; private set; }

        public abstract bool Connect(ITreeComponent fromComponent);

        [Category("Designer")]
        [TypeConverter(typeof(ComponentConverter))]
        public ComponentLayout Layout { get; private set; }
    }

    public enum LeafType
    {
        Amount,
        Number
    }
}