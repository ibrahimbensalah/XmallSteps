using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xania.Steps
{
    public static class StepExtensions
    {
        //public static ComposeStep<TModel, TSubResult, TResult> ForEach<TModel, TSubResult, TResult>(this IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        //{
        //    return new ComposeStep<TModel, TSubResult, TResult>(step1, step2);
        //}

        public static IStep<TRoot> ForEach<TRoot, TResult>(this IStep<TRoot, IEnumerable<TResult>> step1, IStep<TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, IEnumerable<TResult>> ForEach<TRoot, TSubResult, TResult>(this IStep<TRoot, IEnumerable<TSubResult>> step1, IStep<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, IEnumerable<TResult>> Select<TRoot, TSubResult, TResult>(this IStep<TRoot, IEnumerable<TSubResult>> step1, Func<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, TResult> Select<TRoot, TSubResult, TResult>(this IStep<TRoot, TSubResult> step1, Func<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.Select(step2));
        }

        public static IStep<TRoot, IEnumerable<TModel>> Assign<TRoot, TModel, TResult>(this IStep<TRoot, IEnumerable<TModel>> step1, Expression<Func<TModel, TResult>> assignProperty, TResult value)
        {
            return step1.ForEach(new AssignStep<TModel, TResult>(assignProperty, value));
        }

        public static IStep<TRoot, TResult> Invoke<TRoot, TResult>(this IStep<TRoot, TResult> step1, Action<TResult> action)
        {
            return Step.ComposeStep(step1, new SelectStep<TResult, TResult>(m =>
            {
                action(m); return m;
            }));
        }

        public static BranchStep<TRoot, TSubResult, TResult> Branch<TRoot, TSubResult, TResult>(this IStep<TRoot, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            return new BranchStep<TRoot, TSubResult, TResult>(step1, step2);
        }

        public static BranchStep<TRoot, TModel, TResult> Branch<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1,
            Func<IStep<TRoot, TModel>, IStep<TModel, TResult>> branchBuilder)
        {
            var step2 = branchBuilder(step1);
            return step1.Branch(step2);
        }
    }

    public class BranchStep<TRoot, TModel, TResult> : IStep<TModel, TModel>
    {
        private readonly IStep<TRoot, TModel> _step1;
        private readonly IStep<TModel, TResult> _step2;

        public BranchStep(IStep<TRoot, TModel> step1, IStep<TModel, TResult> step2)
        {
            _step1 = step1;
            _step2 = step2;
        }

        public TModel Execute(TModel model)
        {
            _step2.Execute(model);
            return model;
        }

        public IStep<TRoot, TResult> Merge()
        {
            return Step.ComposeStep(_step1, _step2);
        }
    }
}
