namespace Xania.Steps.Core
{
    public static class FunctionExtensions
    {
        public static IFunction<TRoot, TResult> Compose<TRoot, TModel, TResult>(this IFunction<TRoot, TModel> func1,
            IFunction<TModel, TResult> func2)
        {
            return new ComposeFunction<TRoot, TModel, TResult>(func1, func2);
        }
    }
}