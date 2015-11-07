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

        public static IStep<TModel> ForEach<TModel, TResult>(this IStep<TModel, IEnumerable<TResult>> step1, IStep<TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TModel, IEnumerable<TResult>> ForEach<TModel, TSubResult, TResult>(this IStep<TModel, IEnumerable<TSubResult>> step1, IStep<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TModel, IEnumerable<TResult>> Select<TModel, TSubResult, TResult>(this IStep<TModel, IEnumerable<TSubResult>> step1, Func<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.ForEach(step2));
        }

        public static IStep<TModel, TResult> Select<TModel, TSubResult, TResult>(this IStep<TModel, TSubResult> step1, Func<TSubResult, TResult> step2)
        {
            return Step.ComposeStep(step1, Step.Select(step2));
        }

        public static IStep<TModel, IEnumerable<TSubResult>> Assign<TModel, TSubResult, TResult>(this IStep<TModel, IEnumerable<TSubResult>> step1, Expression<Func<TSubResult, TResult>> assignProperty, TResult value)
        {
            return step1.ForEach(new AssignStep<TSubResult, TResult>(assignProperty, value));
        }

        public static IStep<TModel, TResult> Invoke<TModel, TResult>(this IStep<TModel, TResult> step1, Action<TResult> action)
        {
            return Step.ComposeStep(step1, new SelectStep<TResult, TResult>(m =>
            {
                action(m); return m;
            }));
        }

        public static BranchStep<TModel, TSubResult, TResult> Branch<TModel, TSubResult, TResult>(this IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            return new BranchStep<TModel, TSubResult, TResult>(step1, step2);
        }
    }

    public class BranchStep<TModel, TSubResult, TResult> : IStep<TSubResult, TSubResult>
    {
        private readonly IStep<TModel, TSubResult> _step1;
        private readonly IStep<TSubResult, TResult> _step2;

        public BranchStep(IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            _step1 = step1;
            _step2 = step2;
        }

        public TSubResult Execute(TSubResult model)
        {
            _step2.Execute(model);
            return model;
        }

        public IStep<TModel, TResult> Merge()
        {
            return Step.ComposeStep(_step1, _step2);
        }
    }
}
