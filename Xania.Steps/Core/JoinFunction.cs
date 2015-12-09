using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public class JoinFunction<TRoot, TOuter, TInner, TKey, TResult>: IFunction<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunction<TRoot, IEnumerable<TOuter>> _outerFunc;
        private readonly IFunction<TRoot, IEnumerable<TInner>> _innerFunc;
        private readonly Func<TOuter, TKey> _outerKeySelector;
        private readonly Func<TInner, TKey> _innerKeySelector;
        private readonly Func<TOuter, TInner, TResult> _resultSelector;

        public JoinFunction(IFunction<TRoot, IEnumerable<TOuter>> outerFunc, IFunction<TRoot, IEnumerable<TInner>> innerFunc, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            if (outerFunc == null) throw Error.ArgumentNull("outer");
            if (innerFunc == null) throw Error.ArgumentNull("inner");
            if (outerKeySelector == null) throw Error.ArgumentNull("outerKeySelector");
            if (innerKeySelector == null) throw Error.ArgumentNull("innerKeySelector");
            if (resultSelector == null) throw Error.ArgumentNull("resultSelector");

            _outerFunc = outerFunc;
            _innerFunc = innerFunc;
            _outerKeySelector = outerKeySelector;
            _innerKeySelector = innerKeySelector;
            _resultSelector = resultSelector;
        }

        public IEnumerable<TResult> Execute(TRoot root)
        {
            var outer = _outerFunc.Execute(root);
            var inner = _innerFunc.Execute(root);

            return outer.Join(inner, _outerKeySelector, _innerKeySelector, _resultSelector);
        }
    }
}