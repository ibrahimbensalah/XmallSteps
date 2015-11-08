using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps
{
    public class ForEachStep<TModel, TResult> : IStep<IEnumerable<TModel>, IEnumerable<TResult>>
    {
        private readonly IStep<TModel, TResult> _step;

        public ForEachStep(IStep<TModel, TResult> step)
        {
            _step = step;
        }

        public IEnumerable<TResult> Execute(IEnumerable<TModel> models)
        {
            return models.Select(item => _step.Execute(item)).ToArray();
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