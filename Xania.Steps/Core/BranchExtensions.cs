using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.Steps.Core
{
    public static class BranchExtensions
    {
        public static IFunctor<TRoot, TResult> Branch<TRoot, TResult, TBranchResult>(this IFunctor<TRoot, TResult> func,
            Func<IFunctor<TRoot, TResult>, IFunctor<TResult, TBranchResult>> branchBuilder)
        {
            var branchFunc = branchBuilder(func);
            return func.Compose(Functor.Create((TResult m) =>
            {
                branchFunc.Execute(m);
                return m;
            }));
        }
    }
}