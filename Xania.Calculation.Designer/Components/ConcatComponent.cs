using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Xania.Calculation.Designer.Components
{
    public class ConcatComponent : TreeComponent, IEnumerable<NodeComponent>
    {
        public void Add(NodeComponent nodeComponent)
        {
            Connect(nodeComponent);
        }

        [Browsable(false)]
        public override ExpandableDesignerCollection<TreeArgument> Arguments
        {
            get { return base.Arguments; }
        }

        public IEnumerator<NodeComponent> GetEnumerator()
        {
            return Arguments.Select(a => a.Tree).OfType<NodeComponent>().GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("&&");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Connect(ITreeComponent fromComponent)
        {
            var item = fromComponent as NodeComponent;
            if (item != null)
            {
                Arguments.Add(new TreeArgument { Tree = fromComponent });
                return true;
            }

            return false;
        }
    }
}