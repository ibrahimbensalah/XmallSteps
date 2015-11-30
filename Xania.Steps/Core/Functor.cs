using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xania.Steps.Core
{
    public class Functor<TModel, TResult> : IFunctor<TModel, TResult>
    {
        private readonly string _name;

        private readonly Func<TModel, TResult> _func;

        public Functor(string name, Func<TModel, TResult> func)
        {
            _name = name;
            _func = func;
        }

        public TResult Execute(TModel root)
        {
            Console.Write(_name);
            return _func(root);
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class Functor
    {
        public static IdentityFunctor<T> Id<T>()
        {
            return new IdentityFunctor<T>();
        }

        public static IdentityFunctor<IEnumerable<T>> Each<T>()
        {
            return Id<IEnumerable<T>>();
        }

        public static Functor<TModel, TResult> Create<TModel, TResult>(Func<TModel, TResult> func)
        {
            return new Functor<TModel, TResult>(String.Empty, func);
        }
    }

    public class IdentityFunctor<T>: IFunctor<T, T>
    {
        public T Execute(T root)
        {
            Console.Write("1<{0}>", typeof(T).Name);
            return root;
        }
    }
}