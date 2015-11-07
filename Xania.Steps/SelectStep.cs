using System;

namespace Xania.Steps
{
    public class SelectStep<TModel, TResult> : Step<TModel, TResult>
    {
        private readonly Func<TModel, TResult> _selector;

        public SelectStep(Func<TModel, TResult> selector)
        {
            _selector = selector;
        }

        public override TResult Execute(TModel model)
        {
            return _selector(model);
        }
    }
}