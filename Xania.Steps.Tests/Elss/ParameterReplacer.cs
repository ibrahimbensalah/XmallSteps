using System.Linq.Expressions;

namespace Xania.Steps.Tests.Elss
{
    public static class ParameterReplacer
    {
        // Produces an expression identical to 'expression'
        // except with 'source' parameter replaced with 'target' expression.     
        public static Expression Replace(this Expression expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacerVisitor(source, target)
                .VisitAndConvert(expression);
        }

        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _source;
            private readonly Expression _target;

            public ParameterReplacerVisitor
                (ParameterExpression source, Expression target)
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