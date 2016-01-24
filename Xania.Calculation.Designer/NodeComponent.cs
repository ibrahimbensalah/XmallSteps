using System.Collections.Generic;

namespace Xania.Calculation.Designer
{
    public class NodeComponent : ITreeComponent
    {
        public NodeComponent()
        {
            Branches = new List<BranchComponent>();
        }

        public virtual IList<BranchComponent> Branches { get; private set; }
    }

    public class TreeRefComponent : ITreeComponent
    {
        public string Name { get; set; }
    }
}