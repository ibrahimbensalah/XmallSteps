using System;

namespace Xania.Steps
{
    public class LazyValue<TValue>: IValue<TValue>
    {
        private readonly Func<TValue> _valueFunc;

        public LazyValue(Func<TValue> valueFunc)
        {
            _valueFunc = valueFunc;
        }

        public TValue Get()
        {
            return _valueFunc();
        }
    }
}