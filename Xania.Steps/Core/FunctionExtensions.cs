using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public static class FunctionExtensions
    {
        public static IFunction<TRoot, TResult> Compose<TRoot, TModel, TResult>(this IFunction<TRoot, TModel> func1,
            IFunction<TModel, TResult> func2)
        {
            return new ComposeFunction<TRoot, TModel, TResult>(func1, func2);
        }

        public static IFunction<TRoot, TResult> Compose<TRoot, TModel, TResult>(this IFunction<TRoot, TModel> func1,
            Func<TModel, TResult> func2)
        {
            return new ComposeFunction<TRoot, TModel, TResult>(func1, Function.Create(func2));
        }

        public static IFunction<TSource, TResult> Invoke<TSource, TResult>(this IFunction<TSource, TResult> func1, Action<TResult> action)
        {
            var func2 = Function.Create((TResult m) =>
            {
                action(m);
                return m;
            });

            return func1.Compose(func2);
        }

        public static IFunction<TRoot, TResult> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, TSource> func1, Func<TSource, TResult> func2)
        {
            return func1.Compose(Function.Create(func2));
        }

        public static IFunction<TRoot, IEnumerable<TResult>> Join<TRoot, TOuter, TInner, TKey, TResult>(this IFunction<TRoot, IEnumerable<TOuter>> outerFunc, IFunction<TRoot, IEnumerable<TInner>> innerFunc, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return new JoinFunction<TRoot, TOuter, TInner, TKey, TResult>(outerFunc, innerFunc, outerKeySelector,
                innerKeySelector, resultSelector);
        }
    }

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