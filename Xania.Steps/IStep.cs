namespace Xania.Steps
{
    public interface IStep<in TModel>
    {
        void Execute(TModel model);
    }

    public interface IStep<in TModel, out TValue>
    {
        void Execute(TModel model, IStepVisitor<TValue> stepVisitor);
    }

    public interface IValue<out TValue>
    {
        TValue Get();
    }

    public interface IStepVisitor<in TModel>
    {
        void Visit(TModel model);
    }
}