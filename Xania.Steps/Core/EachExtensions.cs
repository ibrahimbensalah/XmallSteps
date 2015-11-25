using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps.Core
{
    public static class EachExtensions
    {
        public static IFunction<TRoot, EachMonad<TResult>> ForEach<TRoot, TModel, TResult>(
            this IFunction<TRoot, EachMonad<TModel>> func1, IFunction<TModel, TResult> elementFunc)
        {
            var bindFunc = new BindFunction<EachMonad<TModel>, EachMonad<TResult>>(m => m.Select(elementFunc.Execute).Each());
            return func1.Compose(bindFunc);
        }

        public static IFunction<TRoot, EachMonad<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, TModel> func1, Func<TModel, IEnumerable<TResult>> func2)
        {
            return func1.Compose(new BindFunction<TModel, EachMonad<TResult>>(e => func2(e).Each()));
        }

        public static IFunction<TRoot, EachMonad<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, EachMonad<TModel>> func1, IFunction<TModel, TResult> func2)
        {
            var aggFunc = new BindFunction<EachMonad<TModel>, EachMonad<TResult>>(m => m.Select(func2.Execute).Each());
            return func1.Compose(aggFunc);
        }

        public static IFunction<TRoot, EachMonad<TResult>> Select<TRoot, TModel, TResult>(
            this IFunction<TRoot, EachMonad<TModel>> func1, Func<TModel, TResult> func2)
        {
            var bindFunction = new BindFunction<EachMonad<TModel>, EachMonad<TResult>>(m => m.Select(func2).Each());
            return func1.Compose(bindFunction);
        }

        public static EachMonad<T> Each<T>(this IEnumerable<T> enumerable)
        {
            return new EachMonad<T>(enumerable);
        }

        public static TResult Execute<TModel, TResult>(IFunction<IEnumerable<TModel>, TResult> func, params TModel[] args)
        {
            return func.Execute(args);
        }
    }
}