using System;
using System.Collections.Generic;
using System.Linq;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public static class EnumerableExtensions
    {
        public static IFunction<TRoot, IEnumerable<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, IEnumerable<TModel>> func1, IFunction<TModel, TResult> func2)
        {
            var bindFunction = Function.Create(string.Format("select {0}", func2), (IEnumerable<TModel> m) => m.Select(func2.Execute));
            return func1.Compose(bindFunction);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, IEnumerable<TSource>> sourceFunc, Func<TSource, TResult> selector)
        {
            var bindFunction = Function.Create(string.Format("select {0}", selector), (IEnumerable<TSource> m) => m.Select(selector));
            return sourceFunc.Compose(bindFunction);
        }

        public static IFunction<TRoot, TResult> Member<TRoot, TSource, TResult>(
            this IFunction<TRoot, TSource> source, Func<TSource, TResult> func2)
        {
            return source.Compose(func2);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectFunction<TRoot, TSource, TCollection, TResult>(source, Function.Create(string.Format("select many {0}", collectionSelector), collectionSelector), resultSelector);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, IEnumerable<TCollection>> collectionFunc, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectManyFunction<TRoot, TSource, TCollection, TResult>(source, Function.Create(string.Format("select many {0}", collectionFunc), collectionFunc), resultSelector);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> SelectMany<TRoot, TSource, TResult>(
            this IFunction<TRoot, IEnumerable<TSource>> func1, Func<TSource, IEnumerable<TResult>> func2)
        {
            var bindFunction = Function.Create(string.Format("select many {0}", func2), (IEnumerable<TSource> source) => source.SelectMany(func2));
            return func1.Compose(bindFunction);
        }

        public static IFunction<TRoot, IEnumerable<TResult>> Join<TRoot, TOuter, TInner, TKey, TResult>(this IFunction<TRoot, IEnumerable<TOuter>> outerFunc, IFunction<TRoot, IEnumerable<TInner>> innerFunc, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return new JoinFunction<TRoot, TOuter, TInner, TKey, TResult>(outerFunc, innerFunc, outerKeySelector,
                innerKeySelector, resultSelector);
        }

        public static IFunction<TRoot, IEnumerable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, IFunction<TSource, bool>> predicateFunc)
        {
            var predicate = predicateFunc(default(TSource));
            var bindFunction = Function.Create(string.Format("where predicate {0}", predicate), (IEnumerable<TSource> x) => x.Where(predicate.Execute));
            return source.Compose(bindFunction);
        }

        public static IFunction<TRoot, IEnumerable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IEnumerable<TSource>> source, Func<TSource, bool> predicateFunc)
        {
            var bindFunction = Function.Create(string.Format("where {0}", predicateFunc), (IEnumerable<TSource> x) => x.Where(predicateFunc));
            return source.Compose(bindFunction);
        }

        public static IFunction<TSource, IEnumerable<TModel>> Execute<TSource, TModel>(
            this IFunction<TSource, IEnumerable<TModel>> func1, Action<TModel> action)
        {
            var bindFunction = Function.Create("for each", (IEnumerable<TModel> m) =>
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

        public static TResult Execute<TModel, TResult>(this IFunction<IQueryable<TModel>, TResult> source, params TModel[] args)
        {
            return source.Execute(args.AsQueryable());
        }
    }
}