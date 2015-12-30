using System;
using Xania.Steps.Core;

namespace Xania.Steps
{
    public static class BranchExtensions
    {
        public static IFunction<TRoot, TResult> Branch<TRoot, TResult, TBranchResult>(this IFunction<TRoot, TResult> func,
            Func<IFunction<TRoot, TResult>, IFunction<TResult, TBranchResult>> branchBuilder)
        {
            var branchFunc = branchBuilder(func);
            return func.Compose(Function.Create((TResult m) =>
            {
                branchFunc.Execute(m);
                return m;
            }));
        }
    }
}