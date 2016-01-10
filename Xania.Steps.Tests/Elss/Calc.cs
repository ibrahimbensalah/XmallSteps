using System;
using System.Linq;
using System.Linq.Expressions;
using Xania.Steps.Core;

namespace Xania.Steps.Tests.Elss
{
    public class Calc<TModel, TResult>: IFunction<TModel, TResult>
    {
        public Calc(Expression<Func<TModel, TResult>> propertyExpression)
        {
            PropertyExpression = propertyExpression;
        }

        public Expression<Func<TModel, TResult>> PropertyExpression { get; private set; }

        public Calc<TModel, TSelect> Select<TSelect>(Expression<Func<TResult, TSelect>> selector)
        {
            var paramExpr = PropertyExpression.Parameters[0];
            var bodyExpr = selector.Body.Replace(selector.Parameters[0], PropertyExpression.Body);
            var lambda = Expression.Lambda<Func<TModel, TSelect>>(bodyExpr, paramExpr);

            return new Calc<TModel, TSelect>(lambda);
        }

        public Calc<TModel, TSelect> SelectMany<TSelect>(Expression<Func<TResult, TSelect>> selector)
        {
            var paramExpr = PropertyExpression.Parameters[0];
            var bodyExpr = selector.Body.Replace(selector.Parameters[0], PropertyExpression.Body);
            var lambda = Expression.Lambda<Func<TModel, TSelect>>(bodyExpr, paramExpr);

            return new Calc<TModel, TSelect>(lambda);
        }

        public static Calc<TModel, TResult> operator +(Calc<TModel, TResult> left, TResult value)
        {
            var body = Expression.Add(left.PropertyExpression.Body, Expression.Constant(value));
            var lambda = Expression.Lambda<Func<TModel, TResult>>(body, left.PropertyExpression.Parameters[0]);

            return new Calc<TModel, TResult>(lambda);
        }

        public static Calc<TModel, TResult> operator & (Calc<TModel, TResult> calc1, Calc<TModel, TResult> calc2)
        {
            return calc1.Merge(calc2);
        }

        public static Calc<TModel, TResult> operator +(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            var expr = Calc.Merge(Expression.Add, left.PropertyExpression, right.PropertyExpression);
            return new Calc<TModel, TResult>(expr);
        }

        public static Calc<TModel, TResult> operator -(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            var expr = Calc.Merge(Expression.Subtract, left.PropertyExpression, right.PropertyExpression);
            return new Calc<TModel, TResult>(expr);
        }

        public static Calc<TModel, TResult> operator *(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            var expr = Calc.Merge(Expression.Multiply, left.PropertyExpression, right.PropertyExpression);
            return new Calc<TModel, TResult>(expr);
        }

        public Calc<TModel, decimal> Set<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute(TModel root)
        {
            var result = PropertyExpression.Compile().Invoke(root);
            Console.WriteLine("{0} -> {1}", PropertyExpression.Body, result);
            return result;
        }
    }

    public static class Calc
    {
        public static Calc<TModel, TModel> Id<TModel>()
        {
            return new Calc<TModel, TModel>(e => e);
        }

        public static Calc<TModel, TMember> Select<TModel, TMember>(Expression<Func<TModel, TMember>> expression)
        {
            return new Calc<TModel, TMember>(expression);
        }

        public static Constant<TValue> Constant<TValue>(Expression<Func<TValue>> expression)
        {
            return new Constant<TValue>(expression);
        }

        public static Calc<TInput, TResult> Merge<TInput, TResult>(this Calc<TInput, TResult> calc1, Calc<TInput, TResult> calc2)
        {
            var mergedExpr = Merge(Merge, calc1.PropertyExpression, calc2.PropertyExpression);
            return Select(mergedExpr);
        }

        public static Calc<TInput, TResult> Merge<TInput, TResult>(this Calc<TInput, TResult> calc1, Calc<Unit, TResult> calc2)
        {
            var bodyExpr = Merge(calc1.PropertyExpression.Body, calc2.PropertyExpression.Body);
            var lambdaExpr = Expression.Lambda<Func<TInput, TResult>>(bodyExpr, calc1.PropertyExpression.Parameters[0]);

            return Select(lambdaExpr);
        }

        private static Expression Merge(Expression expr1, Expression expr2)
        {
            var memberInit1 = expr1 as MemberInitExpression;
            if (memberInit1 == null) 
                throw new ArgumentException("expr1 is not MemberInitExpression");

            var memberInit2 = expr2 as MemberInitExpression;
            if (memberInit2 == null)
                throw new ArgumentException("expr2 is not MemberInitExpression");

            return Expression.MemberInit(memberInit1.NewExpression, memberInit1.Bindings.Concat(memberInit2.Bindings));
        }

        public static Expression<Func<TInput, TResult>> Merge<TInput, TResult>(Func<Expression, Expression, Expression> operation, Expression<Func<TInput, TResult>> left, Expression<Func<TInput, TResult>> right)
        {
            var paramExpr = left.Parameters[0];
            var leftExpr = left.Body;
            var rightExpr = right.Body.Replace(right.Parameters[0], paramExpr);
            var bodyExpr = operation(leftExpr, rightExpr);

            return Expression.Lambda<Func<TInput, TResult>>(bodyExpr, paramExpr);
        }
    }
}