using System;

namespace Xania.Steps.Core
{
    public class SelectFunction<TRoot, TModel, TResult> : IFunction<TRoot, TResult>
    {
        private readonly IFunction<TRoot, TModel> _func;
        private readonly Func<TRoot, TModel, TResult> _selector;

        public SelectFunction(IFunction<TRoot, TModel> func, Func<TRoot, TModel, TResult> selector)
        {
            _func = func;
            _selector = selector;
        }

        public TResult Execute(TRoot root)
        {
            return _selector(root, _func.Execute(root));
        }
    }
}