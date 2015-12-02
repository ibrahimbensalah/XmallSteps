using System;

namespace Xania.Steps.Core
{
    public static class FunctionExtensions
    {
        public static IFunctor<TRoot, TResult> Compose<TRoot, TModel, TResult>(this IFunctor<TRoot, TModel> func1,
            IFunctor<TModel, TResult> func2)
        {
            return new ComposeFunctor<TRoot, TModel, TResult>(func1, func2);
        }

        public static IFunctor<TSource, TResult> Invoke<TSource, TResult>(this IFunctor<TSource, TResult> func1, Action<TResult> action)
        {
            var func2 = Functor.Create((TResult m) =>
            {
                action(m);
                return m;
            });

            return func1.Compose(func2);
        }

        public static IFunctor<TRoot, TResult> Select<TRoot, TSource, TResult>(
            this IFunctor<TRoot, TSource> func1, Func<TSource, TResult> func2)
        {
            return func1.Compose(Functor.Create(func2));
        }

        public static IFunctor<TRoot, TResult> Join<TRoot, TOuter, TInner, TKey, TResult>(this IFunctor<TRoot, TOuter> outer, IFunctor<TRoot, TInner> inner, Func<TRoot, TKey> outerKeySelector, Func<TRoot, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            if (outer == null) throw Error.ArgumentNull("outer");
            if (inner == null) throw Error.ArgumentNull("inner");
            if (outerKeySelector == null) throw Error.ArgumentNull("outerKeySelector");
            if (innerKeySelector == null) throw Error.ArgumentNull("innerKeySelector");
            if (resultSelector == null) throw Error.ArgumentNull("resultSelector");
            throw new Exception();
            // return JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }
    }
}