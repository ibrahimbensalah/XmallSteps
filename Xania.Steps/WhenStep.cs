using System;

namespace Xania.Steps
{
    internal class WhenStep<TModel, TResult>: IStep<TModel, TModel>
    {
        private readonly Func<TModel, bool> _predicate;
        private readonly IStep<TModel, TResult> _trueStep;
        private readonly IStep<TModel, TResult> _falseStep;

        public WhenStep(Func<TModel, bool> predicate, IStep<TModel, TResult> trueStep, IStep<TModel, TResult> falseStep)
        {
            _predicate = predicate;
            _trueStep = trueStep;
            _falseStep = falseStep;
        }

        public TModel Execute(TModel model)
        {
            if (_predicate(model))
            {
                _trueStep.Execute(model, new StepVisitor<TResult>());
            }
            else if (_falseStep != null)
            {
                _falseStep.Execute(model, new StepVisitor<TResult>());
            }
            return model;
        }

        public void Execute(TModel model, IStepVisitor<TModel> stepVisitor)
        {
            Execute(model);
            stepVisitor.Visit(model);
        }
    }
}