using System;

namespace Xania.Steps
{
    public class ComposeStep<TModel, TResult> : IStep<TModel>
    {
        private readonly IStep<TModel, TResult> _step1;
        private readonly IStep<TResult> _step2;

        public ComposeStep(IStep<TModel, TResult> step1, IStep<TResult> step2)
        {
            _step1 = step1;
            _step2 = step2;
        }

        public void Execute(TModel model)
        {
            _step2.Execute(_step1.Execute(model));
        }
    }

    public class ComposeStep<TModel, TSubResult, TResult> : IStep<TModel, TResult>
    {
        private readonly IStep<TModel, TSubResult> _step1;
        private readonly IStep<TSubResult, TResult> _step2;

        public ComposeStep(IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            _step1 = step1;
            _step2 = step2;
        }

        public TResult Execute(TModel model)
        {
            var sub = _step1.Execute(model);
            return _step2.Execute(sub);
        }
    }
}