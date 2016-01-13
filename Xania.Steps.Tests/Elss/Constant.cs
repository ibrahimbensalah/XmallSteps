using System;
using System.Linq.Expressions;

namespace Xania.Steps.Tests.Elss
{
    public class Constant<TModel> : Calc<Unit, TModel>
    {
        public Constant(Expression<Func<TModel>> expression)
            : base(Unit2(expression))
        {
        }

        private static Expression<Func<Unit, TModel>> Unit2(Expression<Func<TModel>> expression)
        {
            return System.Linq.Expressions.Expression.Lambda<Func<Unit, TModel>>(expression.Body, System.Linq.Expressions.Expression.Parameter(typeof (Unit)));
        }

        public TModel Execute()
        {
            return base.Execute(Unit.Any);
        }
    }
}