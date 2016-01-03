using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xania.Steps.Core
{
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

    public class SelectFunction<TRoot, TModel, TResult> : IFunction<TRoot, TResult>
    {
        private readonly IFunction<TRoot, TModel> _func;
        private readonly Func<TRoot, TModel, TResult> _selector;

        public SelectFunction(IFunction<TRoot, TModel> func, Func<TRoot, TModel, TResult> selector)
        {
            _func = func;
            _selector = selector;
        }

        public TResult Execute(TRoot root)
        {
            return _selector(root, _func.Execute(root));
        }
    }

    public class QueryableSelectFunction<TRoot, TSource, TCollection, TResult> : IFunction<TRoot, IQueryable<TResult>>
    {
        private readonly IFunction<TRoot, TSource> _sourceFunc;
        private readonly IFunction<TSource, IQueryable<TCollection>> _collectionFunc;
        private readonly Expression<Func<TCollection, TResult>> _resultSelector;

        public QueryableSelectFunction(IFunction<TRoot, TSource> sourceFunc, IFunction<TSource, IQueryable<TCollection>> collectionFunc, Expression<Func<TCollection, TResult>> resultSelector)
        {
            _sourceFunc = sourceFunc;
            _collectionFunc = collectionFunc;
            _resultSelector = resultSelector;
        }

        public IQueryable<TResult> Execute(TRoot root)
        {
            var s = _sourceFunc.Execute(root);
            var c = _collectionFunc.Execute(s);
            return c.Select(_resultSelector);
        }
    }
}