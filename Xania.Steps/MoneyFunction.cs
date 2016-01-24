using System;
using System.Linq.Expressions;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public class Money<TInput>: IFunction<TInput, decimal>
    {
        private readonly Expression<Func<TInput, decimal>> _valueExpr;

        public Money(Expression<Func<TInput, decimal>> valueExpr)
        {
            _valueExpr = valueExpr;
        }

        public decimal Execute(TInput root)
        {
            return _valueExpr.Compile()(root);
        }

        public static Money<TInput> operator +(Money<TInput> a, Money<TInput> b)
        {
            return new Money<TInput>(input => a.Execute(input) + b.Execute(input));
        }

        public static Money<TInput> operator +(Money<TInput> a, decimal b)
        {
            return new Money<TInput>(input => a.Execute(input) + b);
        }

        public static Money<TInput> operator |(Money<TInput> a, IFunction<decimal, decimal> b)
        {
            return new Money<TInput>(input => a.Compose(b).Execute(input));
        }
    }

    public static class MoneyExtensions
    {
        public static Money<TInput> Money<TRoot, TInput>(this IFunction<TRoot, TInput> input, Expression<Func<TInput, decimal>> valueExpr)
        {
            return new Money<TInput>(valueExpr);
        }
    }
}
