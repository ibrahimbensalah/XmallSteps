using System;
using System.Linq.Expressions;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public static class AssignExtensions
    {
        public static IFunction<TRoot, TModel> Assign<TRoot, TModel, TProperty>(this IFunction<TRoot, TModel> func1, Expression<Func<TModel, TProperty>> propertyExpression, TProperty property)
        {
            var assign = new AssignFunction<TModel, TProperty>( propertyExpression, property);
            return func1.Compose(assign);
        }
    }

    public class AssignFunction<TModel, TProperty>: IFunction<TModel, TModel>
    {
        private readonly Expression<Func<TModel, TProperty>> _propertyExpr;
        private readonly TProperty _value;
        private Action<TModel, TProperty> _assignFunc;

        public AssignFunction(Expression<Func<TModel, TProperty>> propertyExpr, TProperty value)
        {
            _propertyExpr = propertyExpr;
            _value = value;
        }

        public TModel Execute(TModel root)
        {
            var assignFunc = Compile();
            assignFunc(root, _value);

            return root;
        }
        private Action<TModel, TProperty> Compile()
        {
            if (_assignFunc == null)
            {
                var valueParam = Expression.Parameter(typeof(TProperty));
                var assignExpr = Expression.Assign(_propertyExpr.Body, valueParam);
                _assignFunc =
                    Expression.Lambda<Action<TModel, TProperty>>(assignExpr, _propertyExpr.Parameters[0], valueParam)
                        .Compile();
            }
            return _assignFunc;
        }
    }
}
