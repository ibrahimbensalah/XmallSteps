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
    }
}