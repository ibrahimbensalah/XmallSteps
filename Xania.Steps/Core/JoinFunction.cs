using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xania.Steps.Core
{
    public class JoinFunction<TRoot, TOuter, TInner, TKey, TResult> : IFunction<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunction<TRoot, IEnumerable<TOuter>> _outerFunc;
        private readonly IFunction<TRoot, IEnumerable<TInner>> _innerFunc;
        private readonly Func<TOuter, TKey> _outerKeySelector;
        private readonly Func<TInner, TKey> _innerKeySelector;
        private readonly Func<TOuter, TInner, TResult> _resultSelector;

        public JoinFunction(IFunction<TRoot, IEnumerable<TOuter>> outerFunc, IFunction<TRoot, IEnumerable<TInner>> innerFunc, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            if (outerFunc == null) throw new ArgumentNullException("outerFunc");
            if (innerFunc == null) throw new ArgumentNullException("innerFunc");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

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

    public class QueryJoinFunction<TRoot, TOuter, TInner, TKey, TResult> : IFunction<TRoot, IQueryable<TResult>>
    {
        private readonly IFunction<TRoot, IQueryable<TOuter>> _outerFunc;
        private readonly IFunction<TRoot, IQueryable<TInner>> _innerFunc;
        private readonly Expression<Func<TOuter, TKey>> _outerKeySelector;
        private readonly Expression<Func<TInner, TKey>> _innerKeySelector;
        private readonly Expression<Func<TOuter, TInner, TResult>> _resultSelector;

        public QueryJoinFunction(IFunction<TRoot, IQueryable<TOuter>> outerFunc, IFunction<TRoot, IQueryable<TInner>> innerFunc, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            if (outerFunc == null) throw new ArgumentNullException("outerFunc");
            if (innerFunc == null) throw new ArgumentNullException("innerFunc");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            _outerFunc = outerFunc;
            _innerFunc = innerFunc;
            _outerKeySelector = outerKeySelector;
            _innerKeySelector = innerKeySelector;
            _resultSelector = resultSelector;
        }

        public IQueryable<TResult> Execute(TRoot root)
        {
            Console.WriteLine("join using " + root);

            var outer = _outerFunc.Execute(root);
            var inner = _innerFunc.Execute(root);

            return outer.Join(inner, _outerKeySelector, _innerKeySelector, _resultSelector);
        }
    }
}