using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.Steps
{
    public static class LinqExtensions
    {
        public static IStep<TRoot, TInner> Join<TRoot, TModel, TInner, TKey, TResult>(this IStep<TRoot, TModel> outer, IStep<TModel, TInner> inner, Func<TModel, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TModel, TInner, TResult> resultSelector)
        {
            if (outer == null) throw Error.ArgumentNull("outer");
            if (inner == null) throw Error.ArgumentNull("inner");
            if (outerKeySelector == null) throw Error.ArgumentNull("outerKeySelector");
            if (innerKeySelector == null) throw Error.ArgumentNull("innerKeySelector");
            if (resultSelector == null) throw Error.ArgumentNull("resultSelector");

            return outer.Compose(inner);
        }

        //public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IStep<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        //{
        //    if (outer == null) throw Error.ArgumentNull("outer");
        //    if (inner == null) throw Error.ArgumentNull("inner");
        //    if (outerKeySelector == null) throw Error.ArgumentNull("outerKeySelector");
        //    if (innerKeySelector == null) throw Error.ArgumentNull("innerKeySelector");
        //    if (resultSelector == null) throw Error.ArgumentNull("resultSelector");
        //    return JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        //}

        //static IEnumerable<TResult> JoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        //{
        //    Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
        //    foreach (TOuter item in outer)
        //    {
        //        Lookup<TKey, TInner>.Grouping g = lookup.GetGrouping(outerKeySelector(item), false);
        //        if (g != null)
        //        {
        //            for (int i = 0; i < g.count; i++)
        //            {
        //                yield return resultSelector(item, g.elements[i]);
        //            }
        //        }
        //    }
        //}

    }

    public class Error
    {
        public static Exception ArgumentNull(string field)
        {
            return new ArgumentNullException(field);
        }
    }
}
