namespace Xania.Steps
{
    public class BranchStep<TModel, TResult> : IStep<TModel, TModel>
    {
        private readonly IStep<TModel, TResult> _step;

        public BranchStep(IStep<TModel, TResult> step)
        {
            _step = step;
        }

        public virtual TModel Execute(TModel model)
        {
            _step.Execute(model);
            return model;
        }

        public void Execute(TModel model, IStepVisitor<TModel> stepVisitor)
        {
            throw new System.NotImplementedException();
        }
    }
}