namespace Xania.Steps
{
    public class BranchStep<TModel, TResult> : IStep<TModel, TModel>
    {
        private readonly IStep<TModel, TResult> _step;

        public BranchStep(IStep<TModel, TResult> step)
        {
            _step = step;
        }

        public void Execute(TModel model, IStepVisitor<TModel> stepVisitor)
        {
            _step.Execute(model, new StepVisitor<TResult>(r => { }));
            stepVisitor.Visit(model);
        }
    }
}