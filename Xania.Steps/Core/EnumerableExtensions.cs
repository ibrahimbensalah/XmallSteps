using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public static class EnumerableExtensions
    {
        public static IFunction<TRoot, IEnumerable<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, IEnumerable<TModel>> func1, IFunction<TModel, TResult> func2)
        {
            return Select(func1, func2.Execute);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, IEnumerable<TSource>> func1, Func<TSource, TResult> func2)
        {
            var bindFunction = Function.Create((IEnumerable<TSource> m) => m.Select(func2));
            return func1.Compose(bindFunction);
        }

        public static IFunction<TRoot, TResult> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, TSource> source, Func<TRoot, TSource, TResult> func2)
        {
            return new SelectFunction<TRoot, TSource, TResult>(source, func2);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectFunction<TRoot, TSource, TCollection, TResult>(source, Function.Create(collectionSelector), resultSelector);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, IEnumerable<TCollection>> collectionFunc, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectManyFunction<TRoot, TSource, TCollection, TResult>(source, Function.Create(collectionFunc), resultSelector);
        }

        public static IFunction<TRoot, IEnumerable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, IFunction<TSource, bool>> predicateFunc)
        {
            return source.Select(p => p.Where(x => predicateFunc(x).Execute(x)));
        }

        public static IFunction<TRoot, IEnumerable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, bool> predicateFunc)
        {
            return source.Select(p => p.Where(predicateFunc));
        }

        public static IFunction<TSource, IEnumerable<TModel>> ForEach<TSource, TModel>(
            this IFunction<TSource, IEnumerable<TModel>> func1, Action<TModel> action)
        {
            var bindFunction = Function.Create((IEnumerable<TModel> m) =>
            {
                var enumerable = m as TModel[] ?? m.ToArray();
                foreach (var i in enumerable)
                    action(i);

                return enumerable;
            });
            return func1.Compose(bindFunction);
        }

        public static TResult Execute<TModel, TResult>(this IFunction<IEnumerable<TModel>, TResult> func, params TModel[] args)
        {
            return func.Execute(args);
        }
    }
}