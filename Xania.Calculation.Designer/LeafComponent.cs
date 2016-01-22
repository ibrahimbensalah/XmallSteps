namespace Xania.Calculation.Designer
{
    public class LeafComponent : ITreeComponent
    {
        public LeafComponent()
        {
            Type = LeafType.Amount;
        }

        public string Expression { get; set; }
        public LeafType Type { get; set; }
    }

    public enum LeafType
    {
        Amount
    }
}