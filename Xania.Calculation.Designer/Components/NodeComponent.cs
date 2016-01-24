using System.Collections.Generic;

namespace Xania.Calculation.Designer.Components
{
    public class NodeComponent : TreeComponent
    {
        public NodeComponent()
        {
            Branches = new List<BranchComponent>();
        }

        public virtual IList<BranchComponent> Branches { get; private set; }
    }

    public class TreeRefComponent : TreeComponent
    {
        public string Name { get; set; }
    }
}