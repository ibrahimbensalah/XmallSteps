using System;

namespace Xania.Steps.Core
{
    public class BindFunction<TModel, TResult> : IFunction<TModel, TResult>
    {
        private readonly Func<TModel, TResult> _func;

        public BindFunction(Func<TModel, TResult> func)
        {
            _func = func;
        }

        public TResult Execute(TModel model)
        {
            return _func(model);
        }
    }
}
