namespace Xania.Steps
{
    public interface IStep<in TModel>
    {
        void Execute(TModel model);
    }

    public interface IStep<in TModel, out TValue>
    {
        TValue Execute(TModel model);
    }

    public interface IValue<out TValue>
    {
        TValue Get();
    }
}