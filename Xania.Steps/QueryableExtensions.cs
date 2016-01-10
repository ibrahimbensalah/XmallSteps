using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public static class QueryableExtensions
    {
        public static IFunction<TRoot, IQueryable<TResult>> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, IQueryable<TSource>> func1, Expression<Func<TSource, TResult>> selector)
        {
            var bindFunction = Function.Create(string.Format("select {0}", selector), (IQueryable<TSource> m) => m.Select(selector));
            return func1.Compose(bindFunction);
        }

        public static IFunction<TRoot, IQueryable<TResult>> SelectMany<TRoot, TSource, TResult>(
            this IFunction<TRoot, IQueryable<TSource>> func1, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var bindFunction = Function.Create(string.Format("select many {0}", selector), (IQueryable<TSource> m) => m.SelectMany(selector));
            return func1.Compose(bindFunction);
        }

        public static IFunction<TRoot, IQueryable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, IQueryable<TSource>> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionFunc, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            return new QueryableSelectManyFunction<TRoot, TSource, TCollection, TResult>(source, collectionFunc, resultSelector);
        }

        public static IFunction<TRoot, IQueryable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IQueryable<TSource>> source, Expression<Func<TSource, bool>> predicateFunc)
        {
            var bindFunction = Function.Create(string.Format("where {0}", predicateFunc), (IQueryable<TSource> x) => x.Where(predicateFunc));
            return source.Compose(bindFunction);
        }

        public static IFunction<TRoot, IQueryable<TResult>> Join<TRoot, TOuter, TInner, TKey, TResult>(this IFunction<TRoot, IQueryable<TOuter>> outerFunc, IFunction<TRoot, IQueryable<TInner>> innerFunc, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            return new QueryJoinFunction<TRoot, TOuter, TInner, TKey, TResult>(outerFunc, innerFunc, outerKeySelector,
                innerKeySelector, resultSelector);
        }
    }
}