using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Calculation.Designer.Components
{
    public class NodeComponent : TreeComponent, IEnumerable<TreeArgument>
    {
        public string Name { get; set; }

        private IEnumerable<Branch> Branches { get { return Arguments.OfType<Branch>(); } } 

        public override bool Connect(ITreeComponent fromComponent)
        {
            if (Arguments.Any(arg => arg.Tree == fromComponent))
                return false;

            var name = DesignerHelper.GetNewVariableName("prop{0}", Branches.Select(a => a.Name));

            Arguments.Add(new Branch { Name = name, Tree = fromComponent });
            return true;
        }

        public void Add(TreeArgument arg)
        {
            Arguments.Add(arg);
        }

        public IEnumerator<TreeArgument> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("{{ {0} }}", Name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}