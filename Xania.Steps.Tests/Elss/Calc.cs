using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xania.Steps.Core;

namespace Xania.Steps.Tests.Elss
{
    public class Calc<TResult>
    {
        private Lazy<TResult> _value;

        public Expression<Func<TResult>> CalcExpression { get; private set; }

        public Calc(Expression<Func<TResult>> calcExpression)
        {
            CalcExpression = calcExpression;
            _value = new Lazy<TResult>(() => calcExpression.Compile().Invoke());
        }

        public static implicit operator Calc<TResult>(Calc<Unit, TResult> calc)
        {
            return new Calc<TResult>(null);
        }

        public static Calc<TResult> operator &(Calc<TResult> calc1, Calc<TResult> calc2)
        {
            return calc1.SelectMany(calc2);
        }

        public TResult Value { get { return _value.Value; } }
    }

    public class Calc<TModel, TResult>: IFunction<TModel, TResult>
    {
        public Calc(Expression<Func<TModel, TResult>> calcExpression)
        {
            CalcExpression = calcExpression;
        }

        public Expression<Func<TModel, TResult>> CalcExpression { get; private set; }

        public Calc<TModel, TSelect> Select<TSelect>(Expression<Func<TResult, TSelect>> selector)
        {
            var paramExpr = CalcExpression.Parameters[0];
            var bodyExpr = selector.Body.Replace(selector.Parameters[0], CalcExpression.Body);
            var lambda = Expression.Lambda<Func<TModel, TSelect>>(bodyExpr, paramExpr);

            return new Calc<TModel, TSelect>(lambda);
        }

        public Calc<TModel, TSelect> SelectMany<TSelect>(Expression<Func<TResult, TSelect>> selector)
        {
            var paramExpr = CalcExpression.Parameters[0];
            var bodyExpr = selector.Body.Replace(selector.Parameters[0], CalcExpression.Body);
            var lambda = Expression.Lambda<Func<TModel, TSelect>>(bodyExpr, paramExpr);

            return new Calc<TModel, TSelect>(lambda);
        }

        public static Calc<TModel, TResult> operator +(Calc<TModel, TResult> left, TResult value)
        {
            var body = Expression.Add(left.CalcExpression.Body, Expression.Constant(value));
            var lambda = Expression.Lambda<Func<TModel, TResult>>(body, left.CalcExpression.Parameters[0]);

            return new Calc<TModel, TResult>(lambda);
        }

        public static Calc<TModel, TResult> operator &(Calc<TModel, TResult> calc1, Calc<TModel, TResult> calc2)
        {
            return calc1.SelectMany(calc2);
        }

        public static Calc<TModel, TResult> operator +(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            return left.FoldCalculations(Expression.Add, right);
        }

        public static Calc<TModel, TResult> operator -(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            return left.FoldCalculations(Expression.Subtract, right);
        }

        public static Calc<TModel, TResult> operator *(Calc<TModel, TResult> left, Calc<TModel, TResult> right)
        {
            return left.FoldCalculations(Expression.Multiply, right);
        }

        public Calc<TModel, decimal> Set<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute(TModel root)
        {
            var result = CalcExpression.Compile().Invoke(root);
            Console.WriteLine("{0} -> {1}", CalcExpression.Body, result);
            return result;
        }

        private class BindingVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _valuesParam;

            public BindingVisitor(ParameterExpression valuesParam)
            {
                _valuesParam = valuesParam;
            }

            public Expression VisitAndConvert(Expression root)
            {
                return Visit(root);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return base.VisitLambda(node.Update(node.Body, node.Parameters.Concat(new []{_valuesParam})));
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                var bindings = new List<MemberBinding>();
                foreach (var binding in node.Bindings)
                {
                    var assign = binding as MemberAssignment;
                    if (assign != null)
                    {
                        var expr = Expression.Call(Expression.Constant(this), ReportMethodInfo,
                            new[] {_valuesParam, Expression.Constant(assign.Expression), assign.Expression});

                        assign = assign.Update(assign.Expression.Replace(assign.Expression, expr));
                        bindings.Add(assign);
                    }
                    else
                    {
                        bindings.Add(binding);
                    }
                }
                return base.VisitMemberInit(node.Update(node.NewExpression, bindings));
            }

            static BindingVisitor()
            {
                ReportMethodInfo = typeof (BindingVisitor).GetMethods().Single(m => m.Name == "Report");
            }

            private static readonly MethodInfo ReportMethodInfo;

            public static TValue Report<TValue>(IDictionary<Expression, object> values, Expression key, TValue value)
            {
                values[key] = value;
                return value;
            }
        }
    }

    public static class Calc
    {
        public static Calc<TModel, TModel> Id<TModel>()
        {
            return new Calc<TModel, TModel>(e => e);
        }

        public static Calc<TMember> Create<TMember>(Expression<Func<TMember>> expression)
        {
            return new Calc<TMember>(expression);
        }

        public static Calc<TModel, TMember> Select<TModel, TMember>(Expression<Func<TModel, TMember>> expression)
        {
            return new Calc<TModel, TMember>(expression);
        }

        public static Constant<TValue> Constant<TValue>(Expression<Func<TValue>> expression)
        {
            return new Constant<TValue>(expression);
        }

        public static Calc<TModel, TResult> FoldCalculations<TModel, TResult>(this Calc<TModel, TResult> head, Func<Expression, Expression, Expression> f, params Calc<TModel, TResult>[] right)
        {
            var expr = head.CalcExpression.FoldExpressions(f, right.Select(e => e.CalcExpression));
            return new Calc<TModel, TResult>(expr);
        }

        public static Calc<TResult> FoldCalculations<TResult>(this Calc<TResult> head, Func<Expression, Expression, Expression> f, params Calc<TResult>[] right)
        {
            var expr = head.CalcExpression.FoldExpressions(f, right.Select(e => e.CalcExpression));
            return new Calc<TResult>(expr);
        }

        public static Calc<TInput, TResult> SelectMany<TInput, TResult>(this Calc<TInput, TResult> calc1, Calc<TInput, TResult> calc2)
        {
            return calc1.FoldCalculations(CombineResults, calc2);
        }

        public static Calc<TResult> SelectMany<TResult>(this Calc<TResult> calc1, Calc<TResult> calc2)
        {
            return calc1.FoldCalculations(CombineResults, calc2);
        }

        public static Calc<TInput, TResult> SelectMany<TInput, TResult>(this Calc<TInput, TResult> calc1, Calc<Unit, TResult> calc2)
        {
            var bodyExpr = CombineResults(calc1.CalcExpression.Body, calc2.CalcExpression.Body);
            var lambdaExpr = Expression.Lambda<Func<TInput, TResult>>(bodyExpr, calc1.CalcExpression.Parameters[0]);

            return Select(lambdaExpr);
        }

        private static Expression CombineResults(Expression expr1, Expression expr2)
        {
            var memberInit1 = expr1 as MemberInitExpression;
            if (memberInit1 == null) 
                throw new ArgumentException("expr1 is not MemberInitExpression");

            var memberInit2 = expr2 as MemberInitExpression;
            if (memberInit2 == null)
                throw new ArgumentException("expr2 is not MemberInitExpression");

            return Expression.MemberInit(memberInit1.NewExpression, memberInit1.Bindings.Concat(memberInit2.Bindings));
        }
    }
}