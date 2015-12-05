using System;

namespace Xania.Steps.Core
{
    public class ComposeFunction<TRoot, TModel, TResult> : IFunction<TRoot, TResult>
    {
        private readonly IFunction<TRoot, TModel> _func1;
        private readonly IFunction<TModel, TResult> _func2;

        public ComposeFunction(IFunction<TRoot, TModel> func1, IFunction<TModel, TResult> func2)
        {
            _func1 = func1;
            _func2 = func2;
        }

        public TResult Execute(TRoot root)
        {
            var a = _func1.Execute(root);
            Console.Write(" | ");
            return _func2.Execute(a);
        }
    }
}