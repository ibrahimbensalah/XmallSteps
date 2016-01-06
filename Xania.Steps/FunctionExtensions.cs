using System;
using System.Collections.Generic;
using System.Linq;
using Xania.Steps.Core;

namespace Xania.Steps
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
            return new ComposeFunction<TRoot, TModel, TResult>(func1, Function.Create(func2.ToString(), func2));
        }

        public static IFunction<TSource, TResult> Invoke<TSource, TResult>(this IFunction<TSource, TResult> func1, Action<TResult> action)
        {
            var func2 = Function.Create(string.Format("invoke {0}", action), (TResult m) =>
            {
                action(m);
                return m;
            });

            return func1.Compose(func2);
        }
    }
}