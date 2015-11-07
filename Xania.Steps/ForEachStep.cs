using System.Collections.Generic;

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
            foreach (var item in models)
                yield return _step.Execute(item);
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