using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps
{
    public class EachStep<TModel> : IStep<IEnumerable<TModel>, TModel>
    {
        public void Execute(IEnumerable<TModel> model, IStepVisitor<TModel> stepVisitor)
        {
            foreach(var m in model)
                stepVisitor.Visit(m);
        }
    }

    public class ForEachStep<TModel> : IStep<IEnumerable<TModel>>
    {
        private readonly IStep<TModel> _step;

        public ForEachStep(IStep<TModel> step)
        {
            _step = step;
        }

        public void Execute(IEnumerable<TModel> models)
        {
            foreach (var item in models)
                _step.Execute(item);
        }
    }
}