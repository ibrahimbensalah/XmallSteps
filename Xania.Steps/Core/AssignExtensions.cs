using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xania.Steps.Core
{
    public static class AssignExtensions
    {
        public static IFunctor<TRoot, TModel> Assign<TRoot, TModel, TProperty>(this IFunctor<TRoot, TModel> func1, Expression<Func<TModel, TProperty>> propertyExpression, TProperty property)
        {
            var assign = new AssignFunctor<TModel, TProperty>( propertyExpression, property);
            return func1.Compose(assign);
        }
    }

    public class AssignFunctor<TModel, TProperty>: IFunctor<TModel, TModel>
    {
        private readonly Expression<Func<TModel, TProperty>> _propertyExpr;
        private readonly TProperty _value;
        private Action<TModel, TProperty> _assignFunc;

        public AssignFunctor(Expression<Func<TModel, TProperty>> propertyExpr, TProperty value)
        {
            _propertyExpr = propertyExpr;
            _value = value;
        }

        public TModel Execute(TModel root)
        {
            Console.Write("[assign]");
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
