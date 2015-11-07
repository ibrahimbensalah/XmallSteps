using System.Collections;
using System.Collections.Generic;

namespace Xania.Steps
{
    public class SequenceStep<TModel> : Step<TModel>, IEnumerable<IStep<TModel>>
    {
        private readonly IList<IStep<TModel>> _childSteps;

        public SequenceStep()
        {
            _childSteps = new List<IStep<TModel>>();
        }

        public void Add(IStep<TModel> step)
        {
            _childSteps.Add(step);
        }

        public IEnumerator<IStep<TModel>> GetEnumerator()
        {
            return _childSteps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void Execute(TModel model)
        {
            foreach (var step in _childSteps)
            {
                step.Execute(model);
            }
        }
    }
}