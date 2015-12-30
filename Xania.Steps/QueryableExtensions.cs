using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public static class QueryableExtensions
    {
        public static IFunction<TRoot, IQueryable<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, IQueryable<TModel>> func1, IFunction<TModel, TResult> func2)
        {
            return Select(func1, func2.Execute);
        }

        public static IFunction<TRoot, IQueryable<TResult>> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, IQueryable<TSource>> func1, Func<TSource, TResult> func2)
        {
            throw new NotImplementedException();
        }

        public static IFunction<TRoot, TResult> Select<TRoot, TSource, TResult>(
            this IFunction<TRoot, TSource> source, Func<TRoot, TSource, TResult> func2)
        {
            throw new NotImplementedException();
        }

        public static IFunction<TRoot, IQueryable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, TSource> source, Func<TSource, IQueryable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IFunction<TRoot, IQueryable<TResult>> SelectMany<TRoot, TSource, TCollection, TResult>(
            this IFunction<TRoot, IQueryable<TSource>> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionFunc, Func<TSource, TCollection, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IFunction<TRoot, IQueryable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IQueryable<TSource>> source, Func<TSource, IFunction<TSource, bool>> predicateFunc)
        {
            return source.Select(p => p.Where(x => predicateFunc(x).Execute(x)));
        }

        public static IFunction<TRoot, IQueryable<TSource>> Where<TRoot, TSource>(
            this IFunction<TRoot, IQueryable<TSource>> source, Func<TSource, bool> predicateFunc)
        {
            throw new NotImplementedException();
        }

        public static IFunction<TSource, IQueryable<TModel>> ForEach<TSource, TModel>(
            this IFunction<TSource, IQueryable<TModel>> func1, Action<TModel> action)
        {
            throw new NotImplementedException();
        }

        //public static TResult Execute<TModel, TResult>(this IFunction<IQueryable<TModel>, TResult> func, params TModel[] args)
        //{
        //    return func.Execute(args);
        //}
    }
}