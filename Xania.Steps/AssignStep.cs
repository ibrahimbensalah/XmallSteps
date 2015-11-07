using System;
using System.Linq.Expressions;

namespace Xania.Steps
{
    public class AssignStep<TModel, TValue>: Step<TModel, TModel>
    {
        private readonly Expression<Func<TModel, TValue>> _propertyExpr;
        private readonly TValue _value;
        private Action<TModel, TValue> _assignFunc;

        public AssignStep(Expression<Func<TModel, TValue>> propertyExpr, TValue value)
        {
            _propertyExpr = propertyExpr;
            _value = value;
        }

        public override TModel Execute(TModel model)
        {
            var assignFunc = Compile();
            assignFunc(model, _value);

            return model;
        }

        private Action<TModel, TValue> Compile()
        {
            if (_assignFunc == null)
            {
                var valueParam = Expression.Parameter(typeof (TValue));
                var assignExpr = Expression.Assign(_propertyExpr.Body, valueParam);
                _assignFunc =
                    Expression.Lambda<Action<TModel, TValue>>(assignExpr, _propertyExpr.Parameters[0], valueParam)
                        .Compile();
            }
            return _assignFunc;
        }
    }
}