using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xania.Steps.Core
{
    public class Function<TModel, TResult> : IFunction<TModel, TResult>
    {
        private readonly string _name;

        private readonly Func<TModel, TResult> _func;

        public Function(string name, Func<TModel, TResult> func)
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

    public class ExpressionFunction<TModel, TResult> : IFunction<TModel, TResult>
    {
        private readonly string _name;

        private readonly Expression<Func<TModel, TResult>> _func;

        public ExpressionFunction(string name, Expression<Func<TModel, TResult>> func)
        {
            _name = name;
            _func = func;
        }

        public TResult Execute(TModel root)
        {
            Console.Write(_name);
            return _func.Compile()(root);
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class Function
    {
        public static Identity<T> Id<T>()
        {
            return new Identity<T>();
        }

        public static Identity<IEnumerable<T>> Each<T>()
        {
            return Id<IEnumerable<T>>();
        }

        public static Identity<IQueryable<T>> Query<T>()
        {
            return Id<IQueryable<T>>();
        }

        public static Function<TModel, TResult> Create<TModel, TResult>(Func<TModel, TResult> func)
        {
            return new Function<TModel, TResult>(String.Empty, func);
        }

        public static ExpressionFunction<TModel, TResult> FromExpression<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            return new ExpressionFunction<TModel, TResult>(String.Empty, func);
        }
    }

    public class Identity<T>: IFunction<T, T>
    {
        public T Execute(T root)
        {
            Console.Write("1<{0}>", typeof(T).Name);
            return root;
        }
    }
}