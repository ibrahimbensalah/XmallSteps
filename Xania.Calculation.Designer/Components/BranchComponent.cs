namespace Xania.Calculation.Designer.Components
{
    public class BranchComponent
    {
        public string Name { get; set; }

        public bool Many { get; set; }

        public ITreeComponent Tree { get; set; }
        
        public string Path { get; set; }
    }
}