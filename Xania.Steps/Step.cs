using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xania.Steps
{
    public class Step
    {
        public static SelectStep<TContainer, TModel> Select<TContainer, TModel>(Func<TContainer, TModel> modelSelector)
        {
            return new SelectStep<TContainer, TModel>(modelSelector);
        }

        public static AggregateStep<TValue> Aggregate<TValue>(Func<IEnumerable<TValue>, TValue> func)
        {
            return new AggregateStep<TValue>(func);
        }

        public static AssignStep<TModel, TValue> Assign<TModel, TValue>(
            Expression<Func<TModel, TValue>> propertyExpression, TValue value)
        {
            return new AssignStep<TModel, TValue>(propertyExpression, value);
        }

        public static EachStep<TModel> Each<TModel>()
        {
            return new EachStep<TModel>();
        }

        public static SequenceStep<TModel> Sequence<TModel>(params IStep<TModel>[] steps)
        {
            var sequence = new SequenceStep<TModel>();

            foreach (var step in steps)
                sequence.Add(step);

            return sequence;
        }

        public static ComposeStep<TModel, TSubResult, TResult> Compose<TModel, TSubResult, TResult>(
            IStep<TModel, TSubResult> step1, IStep<TSubResult, TResult> step2)
        {
            return new ComposeStep<TModel, TSubResult, TResult>(step1, step2);
        }

        public static ComposeStep<TModel, TResult> Compose<TModel, TResult>(
            IStep<TModel, TResult> step1, IStep<TResult> step2)
        {
            return new ComposeStep<TModel, TResult>(step1, step2);
        }

        public static IRoot<TModel> Root<TModel>()
        {
            return new IdentityStep<TModel>();
        }
    }

    public interface IRoot<TModel> : IStep<TModel, TModel>
    {
    }

    public class IdentityStep<TModel> : IRoot<TModel>
    {
        public TModel Execute(TModel model)
        {
            return model;
        }

        public void Execute(TModel model, IStepVisitor<TModel> stepVisitor)
        {
            stepVisitor.Visit(model);
        }
    }

    public abstract class Step<TModel, TResult> : IStep<TModel, TResult>
    {
        public abstract void Execute(TModel model, IStepVisitor<TResult> stepVisitor);

        public static implicit operator Step<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            return new SelectStep<TModel, TResult>(func.Compile());
        }
    }

    public abstract class Step<TModel> : IStep<TModel>
    {
        public abstract void Execute(TModel model);
    }
}