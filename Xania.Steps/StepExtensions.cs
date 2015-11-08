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
        public static IStep<TModel, TResult> Compose<TModel, TSubResult, TResult>(this IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            return Step.Compose(step1, step2);
        }

        public static IStep<TRoot> ForEach<TRoot, TResult>(this IStep<TRoot, IEnumerable<TResult>> step1, IStep<TResult> step2)
        {
            return Step.Compose(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, IEnumerable<TResult>> ForEach<TRoot, TSubResult, TResult>(this IStep<TRoot, IEnumerable<TSubResult>> step1, IStep<TSubResult, TResult> step2)
        {
            return Step.Compose(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, IEnumerable<TResult>> Select<TRoot, TSubResult, TResult>(this IStep<TRoot, IEnumerable<TSubResult>> step1, Func<TSubResult, TResult> step2)
        {
            return Step.Compose(step1, Step.ForEach(step2));
        }

        public static IStep<TRoot, TResult> Select<TRoot, TSubResult, TResult>(this IStep<TRoot, TSubResult> step1, Func<TSubResult, TResult> step2)
        {
            return Step.Compose(step1, Step.Select(step2));
        }

        public static IStep<TRoot, IEnumerable<TModel>> Assign<TRoot, TModel, TResult>(this IStep<TRoot, IEnumerable<TModel>> step1, Expression<Func<TModel, TResult>> assignProperty, TResult value)
        {
            return step1.ForEach(new AssignStep<TModel, TResult>(assignProperty, value));
        }

        public static IStep<TRoot, TModel> Assign<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1, Expression<Func<TModel, TResult>> assignProperty, TResult value)
        {
            return step1.Compose(new AssignStep<TModel, TResult>(assignProperty, value));
        }

        public static IStep<TRoot, TResult> Invoke<TRoot, TResult>(this IStep<TRoot, TResult> step1, Action<TResult> action)
        {
            return Step.Compose(step1, new SelectStep<TResult, TResult>(m =>
            {
                action(m); return m;
            }));
        }

        public static BranchStep<TModel, TResult> Branch<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1, IStep<TModel, TResult> step2)
        {
            return new BranchStep<TModel, TResult>(step2);
        }

        public static BranchStep<TModel, TResult> Branch<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1,
            Func<IStep<TRoot, TModel>, IStep<TModel, TResult>> branchBuilder)
        {
            var step2 = branchBuilder(step1);
            return step1.Branch(step2);
        }

        public static BranchStep<TSubModel, TResult> Branch<TRoot, TSubModel, TModel, TResult>(this IStep<TRoot, TSubModel> step1,
            Func<TSubModel, TModel> modelSelector,
            Func<IRoot<TModel>, IStep<TModel, TResult>> branchBuilder)
        {
            var step2 = branchBuilder(Step.Root<TModel>());
            var selectStep = Step.Root<TSubModel>().Select(modelSelector);
            return step1.Branch(r => selectStep.Compose(step2));
        }

        public static IStep<TRoot, TModel> When<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1,
            Func<TModel, bool> predicate,
            Func<IStep<TRoot, TModel>, IStep<TModel, TResult>> trueBranchBuilder,
            Func<IStep<TRoot, TModel>, IStep<TModel, TResult>> falseBranchBuilder = null)
        {
            var trueBranch = trueBranchBuilder(step1);
            var falseBranch = falseBranchBuilder == null ? null : falseBranchBuilder(step1);
            return step1.When(predicate, trueBranch, falseBranch);
        }

        private static IStep<TRoot, TModel> When<TRoot, TModel, TResult>(this IStep<TRoot, TModel> step1, Func<TModel, bool> predicate, IStep<TModel, TResult> trueStep, IStep<TModel, TResult> falseStep)
        {
            return step1.Compose(new WhenStep<TModel, TResult>(predicate, trueStep, falseStep));
        }
    }
}
