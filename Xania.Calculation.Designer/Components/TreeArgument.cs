using System.ComponentModel;

namespace Xania.Calculation.Designer.Components
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TreeArgument
    {
        public string Name { get; set; }

        [Browsable(false)]
        public ITreeComponent Tree { get; set; }
    }

    public class Formula : TreeArgument
    {
    }

    public class Branch : TreeArgument
    {
        public bool Many { get; set; }
        public string Path { get; set; }
    }

    public class Repository : TreeArgument
    {
        public string Query { get; set; }
    }
}