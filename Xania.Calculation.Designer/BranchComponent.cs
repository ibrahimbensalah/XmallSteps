namespace Xania.Calculation.Designer
{
    public class BranchComponent
    {
        public string Name { get; set; }

        public bool IsList { get; set; }

        public ITreeComponent Tree { get; set; }
        public string Path { get; set; }
    }
}