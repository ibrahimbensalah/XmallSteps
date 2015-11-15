﻿using System;

namespace Xania.Steps
{
    public class SelectStep<TModel, TResult> : Step<TModel, TResult>
    {
        private readonly Func<TModel, TResult> _selector;

        public SelectStep(Func<TModel, TResult> selector)
        {
            _selector = selector;
        }

        public override void Execute(TModel model, IStepVisitor<TResult> stepVisitor)
        {
            stepVisitor.Visit(_selector(model));
        }
    }
}