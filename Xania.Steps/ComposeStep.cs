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
            throw new InvalidOperationException();
            // _step2.Execute(_step1.Execute(model));
        }
    }

    public class ComposeStep<TRoot, TModel, TResult> : IStep<TRoot, TResult>
    {
        private readonly IStep<TRoot, TModel> _step1;
        private readonly IStep<TModel, TResult> _step2;

        public ComposeStep(IStep<TRoot, TModel> step1, IStep<TModel, TResult> step2)
        {
            _step1 = step1;
            _step2 = step2;
        }

        public void Execute(TRoot model, IStepVisitor<TResult> stepVisitor)
        {
            var stepVisitor2 = new StepVisitor<TModel>(m => _step2.Execute(m, stepVisitor));
            _step1.Execute(model, stepVisitor2);
        }

    }

    public class StepVisitor<T>: IStepVisitor<T>
    {
        private readonly Action<T> _action;

        public StepVisitor()
        {
        }

        public StepVisitor(Action<T> action)
        {
            _action = action;
        }

        public void Visit(T model)
        {
            if (_action != null)
                _action(model);
        }
    }
}