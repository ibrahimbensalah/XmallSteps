using System;
using System.Collections.Generic;

namespace Xania.Steps.Core
{
    public class Functor<TModel, TResult> : IFunction<TModel, TResult>
    {
        private readonly Func<TModel, TResult> _func;

        public Functor(Func<TModel, TResult> func)
        {
            _func = func;
        }

        public TResult Execute(TModel model)
        {
            return _func(model);
        }
    }

    public class Functor
    {
        public static IdentityFunctor<T> Id<T>()
        {
            return new IdentityFunctor<T>();
        }

        public static IFunction<IEnumerable<T>, EachMonad<T>> Each<T>()
        {
            return Id<IEnumerable<T>>().Select(e => e.Each());
        }
    }

    public class IdentityFunctor<T>: IFunction<T, T>
    {
        public T Execute(T model)
        {
            return model;
        }
    }
}