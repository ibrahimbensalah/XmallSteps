using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var func2 = Function.Create(String.Format("invoke {0}", action), (TResult m) =>
            {
                action(m);
                return m;
            });

            return func1.Compose(func2);
        }

        public static Expression<Func<TInput, TResult>> FoldExpressions<TInput, TResult>(this Expression<Func<TInput, TResult>> head, Func<Expression, Expression, Expression> f, params Expression<Func<TInput, TResult>>[] tail)
        {
            return FoldExpressions(head, f, tail.AsEnumerable());
        }

        public static Expression<Func<TInput, TResult>> FoldExpressions<TInput, TResult>(this Expression<Func<TInput, TResult>> head, Func<Expression, Expression, Expression> f, IEnumerable<Expression<Func<TInput, TResult>>> tail)
        {
            var paramExpr = head.Parameters[0];
            var bodyExpr = tail.Select(right => right.Body.Replace(right.Parameters[0], paramExpr))
                .Aggregate(head.Body, (current, rightExpr) => f(current, rightExpr));

            return Expression.Lambda<Func<TInput, TResult>>(bodyExpr, paramExpr);
        }

        public static Expression<Func<TResult>> FoldExpressions<TInput, TResult>(
            this Expression<Func<TResult>> head, Func<Expression, Expression, Expression> f,
            params Expression<Func<TResult>>[] tail)
        {
            return FoldExpressions(head, f, tail.AsEnumerable());
        }

        public static Expression<Func<TResult>> FoldExpressions<TResult>(this Expression<Func<TResult>> head, Func<Expression, Expression, Expression> f, IEnumerable<Expression<Func<TResult>>> tail)
        {
            var bodyExpr = tail.Select(right => right.Body)
                .Aggregate(head.Body, (current, rightExpr) => f(current, rightExpr));

            return Expression.Lambda<Func<TResult>>(bodyExpr);
        }
    }

    public static class ParameterReplacer
    {
        // Produces an expression identical to 'expression'
        // except with 'source' parameter replaced with 'target' expression.     
        public static Expression Replace(this Expression expression, Expression source, Expression target)
        {
            return new ParameterReplacerVisitor(source, target)
                .VisitAndConvert(expression);
        }

        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly Expression _source;
            private readonly Expression _target;

            public ParameterReplacerVisitor
                (Expression source, Expression target)
            {
                _source = source;
                _target = target;
            }

            internal Expression VisitAndConvert(Expression root)
            {
                return Visit(root);
            }

            public override Expression Visit(Expression node)
            {
                return _source == node ? _target : base.Visit(node);
            }
        }
    }
}