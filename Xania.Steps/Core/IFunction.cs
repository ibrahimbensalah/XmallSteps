namespace Xania.Steps.Core
{
    public interface IFunction<in TSource, out TResult>
    {
        TResult Execute(TSource root);
    }
}