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
}