using System;

namespace Xania.Calculation.Designer.Components
{
    public class TreeRefComponent : TreeComponent
    {
        public string Name { get; set; }
        public override bool Connect(ITreeComponent fromComponent)
        {
            throw new NotImplementedException();
        }
    }
}