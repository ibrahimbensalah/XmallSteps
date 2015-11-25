namespace Xania.Steps.Core
{
    public interface IFunction<in TModel, out TResult>
    {
        TResult Execute(TModel model);
    }
}