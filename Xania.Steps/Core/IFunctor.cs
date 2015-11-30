namespace Xania.Steps.Core
{
    public interface IFunctor<in TSource, out TResult>
    {
        TResult Execute(TSource root);
    }
}