using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

    public class QueryableSelectManyFunction<TRoot, TCollection, TResult> : IFunction<TRoot, IQueryable<TResult>>
    {
        private readonly IFunction<TRoot, IQueryable<TCollection>> _func1;
        private readonly Expression<Func<TCollection, IEnumerable<TResult>>> _collectionSelector;

        public QueryableSelectManyFunction(IFunction<TRoot, IQueryable<TCollection>> func1, Expression<Func<TCollection, IEnumerable<TResult>>> collectionSelector)
        {
            _func1 = func1;
            _collectionSelector = collectionSelector;
        }

        public IQueryable<TResult> Execute(TRoot root)
        {
            return _func1.Execute(root).SelectMany(_collectionSelector);
        }
    }

    public class QueryableSelectManyFunction<TRoot, TSource, TCollection, TResult> : IFunction<TRoot, IQueryable<TResult>>
    {
        private readonly IFunction<TRoot, IQueryable<TSource>> _sourceFunc;
        private readonly Expression<Func<TSource, IEnumerable<TCollection>>> _collectionFunc;
        private readonly Expression<Func<TSource, TCollection, TResult>> _resultSelector;

        public QueryableSelectManyFunction(IFunction<TRoot, IQueryable<TSource>> sourceFunc, Expression<Func<TSource, IEnumerable<TCollection>>> collectionFunc, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            if (collectionFunc == null)
                throw new ArgumentNullException("collectionFunc");

            _sourceFunc = sourceFunc;
            _collectionFunc = collectionFunc;
            _resultSelector = resultSelector;
        }

        public IQueryable<TResult> Execute(TRoot root)
        {
            return _sourceFunc.Execute(root).SelectMany(_collectionFunc, _resultSelector);
        }
    }
}