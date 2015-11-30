using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public static class EachExtensions
    {
        public static IFunctor<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TModel, TResult>(
            this IFunctor<TRoot, TModel> func1, Func<TModel, IEnumerable<TResult>> func2)
        {
            return func1.Compose(Functor.Create(func2));
        }

        public static IFunctor<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TModel, TResult>(
            this IFunctor<TRoot, TSource> func1, Func<TSource, IEnumerable<TModel>> func2, Func<TRoot, TModel, TResult> selector)
        {
            return new SelectManyFunctor<TRoot, TModel, TResult>(func1.Compose(Functor.Create(func2)), selector);

        }

        public static IFunctor<TRoot, IEnumerable<TResult>> Select<TRoot, TModel, TResult>(
            this IFunctor<TRoot, IEnumerable<TModel>> func1, IFunctor<TModel, TResult> func2)
        {
            return Select(func1, func2.Execute);
        }

        public static IFunctor<TRoot, IEnumerable<TResult>> Select<TRoot, TSource, TResult>(
            this IFunctor<TRoot, IEnumerable<TSource>> func1, Func<TSource, TResult> func2)
        {
            var bindFunction = Functor.Create((IEnumerable<TSource> m) => m.Select(func2));
            return func1.Compose(bindFunction);
        }

        public static IFunctor<TSource, IEnumerable<TModel>> ForEach<TSource, TModel>(
            this IFunctor<TSource, IEnumerable<TModel>> func1, Action<TModel> action)
        {
            var bindFunction = Functor.Create((IEnumerable<TModel> m) =>
            {
                var enumerable = m as TModel[] ?? m.ToArray();
                foreach (var i in enumerable)
                    action(i);

                return enumerable;
            });
            return func1.Compose(bindFunction);
        }

        public static TResult Execute<TModel, TResult>(this IFunctor<IEnumerable<TModel>, TResult> func, params TModel[] args)
        {
            return func.Execute(args);
        }
    }

    internal class SelectManyFunctor<TRoot, TModel, TResult> : IFunctor<TRoot, IEnumerable<TResult>>
    {
        private readonly IFunctor<TRoot, IEnumerable<TModel>> _func;
        private readonly Func<TRoot, TModel, TResult> _selector;

        public SelectManyFunctor(IFunctor<TRoot, IEnumerable<TModel>> func, Func<TRoot, TModel, TResult> selector)
        {
            _func = func;
            _selector = selector;
        }

        public IEnumerable<TResult> Execute(TRoot root)
        {
            return _func.Select(model => _selector(root, model)).Execute(root);
        }
    }
}