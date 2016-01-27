using System.ComponentModel;

namespace Xania.Calculation.Designer.Components
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TreeArgument
    {
        public string Name { get; set; }

        public bool Many { get; set; }

        [Browsable(false)]
        public ITreeComponent Tree { get; set; }
        
        public string Path { get; set; }
    }
}