using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public class SelectManyFunction<TRoot, TCollection, TResult> : IFunction<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunction<TRoot, IEnumerable<TCollection>> _func1;
        private readonly IFunction<TCollection, IEnumerable<TResult>> _func2;

        public SelectManyFunction(IFunction<TRoot, IEnumerable<TCollection>> func1, IFunction<TCollection, IEnumerable<TResult>> func2)
        {
            _func1 = func1;
            _func2 = func2;
        }

        public IEnumerable<TResult> Execute(TRoot root)
        {
            return _func1.Execute(root).SelectMany(_func2.Execute);
        }
    }

    public class SelectFunction<TRoot, TSource, TCollection, TResult> : IFunction<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunction<TRoot, TSource> _sourceFunc;
        private readonly IFunction<TSource, IEnumerable<TCollection>> _collectionFunc;
        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        public SelectFunction(IFunction<TRoot, TSource> sourceFunc, IFunction<TSource, IEnumerable<TCollection>> collectionFunc, Func<TSource, TCollection, TResult> resultSelector)
        {
            _sourceFunc = sourceFunc;
            _collectionFunc = collectionFunc;
            _resultSelector = resultSelector;
        }

        public IEnumerable<TResult> Execute(TRoot root)
        {
            var s = _sourceFunc.Execute(root);
            var c = _collectionFunc.Execute(s);
            return c.Select(x => _resultSelector(s, x));
        }
    }

    public class SelectManyFunction<TRoot, TSource, TCollection, TResult> : IFunction<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunction<TRoot, IEnumerable<TSource>> _sourceFunc;
        private readonly IFunction<TSource, IEnumerable<TCollection>> _collectionFunc;
        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        public SelectManyFunction(IFunction<TRoot, IEnumerable<TSource>> sourceFunc, IFunction<TSource, IEnumerable<TCollection>> collectionFunc, Func<TSource, TCollection, TResult> resultSelector)
        {
            _sourceFunc = sourceFunc;
            _collectionFunc = collectionFunc;
            _resultSelector = resultSelector;
        }

        public IEnumerable<TResult> Execute(TRoot root)
        {
            return from s in _sourceFunc.Execute(root)
                from c in _collectionFunc.Execute(s)
                select _resultSelector(s, c);
        }
    }
}