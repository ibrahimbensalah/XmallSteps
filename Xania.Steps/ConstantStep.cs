namespace Xania.Steps
{
    public class ConstantStep<TValue>: IValue<TValue>
    {
        private readonly TValue _value;

        public ConstantStep(TValue value)
        {
            _value = value;
        }

        public TValue Get()
        {
            return _value;
        }
    }
}