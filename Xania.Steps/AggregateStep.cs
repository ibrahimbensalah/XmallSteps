using System;
using System.Collections.Generic;

namespace Xania.Steps
{
    public class AggregateStep<TValue> : Step<IEnumerable<TValue>, TValue>
    {
        private readonly Func<IEnumerable<TValue>, TValue> _aggregateFunc;

        public AggregateStep(Func<IEnumerable<TValue>, TValue> aggregateFunc)
        {
            _aggregateFunc = aggregateFunc;
        }

        public override void Execute(IEnumerable<TValue> model, IStepVisitor<TValue> stepVisitor)
        {
            stepVisitor.Visit(_aggregateFunc(model));
        }
    }
}