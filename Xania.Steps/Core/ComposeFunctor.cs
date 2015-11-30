using System;

namespace Xania.Steps.Core
{
    public class ComposeFunctor<TRoot, TModel, TResult> : IFunctor<TRoot, TResult>
    {
        private readonly IFunctor<TRoot, TModel> _func1;
        private readonly IFunctor<TModel, TResult> _func2;

        public ComposeFunctor(IFunctor<TRoot, TModel> func1, IFunctor<TModel, TResult> func2)
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