using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// ReSharper disable All
namespace Xania.Steps.Core
{
    public static class MonadExtensions
    {
        private static Func<A, IMonad<M, C>> Bind<M, A, B, C>(this Func<A, IMonad<M, B>> func1, Func<B, IMonad<M, C>> func2)
        {
            return a => func1(a).Bind(func2);
        }

        public static EnumerableMonad<T> ToMonad<T>(this IEnumerable<T> enumerable)
        {
            return new EnumerableMonad<T>(enumerable);
        }  

        private static void Test()
        {
            Func<Organisation, EnumerableMonad<Person>> f = o => o.Persons.ToMonad();
            Func<Person, EnumerableMonad<int>> g = p => p.Grades.ToMonad();
            var h = f.Bind(g);
            var r = h(new Organisation());
        }
    }

    public interface IMonad<M, T>
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        IMonad<M, R> Bind<R>(Func<T, IMonad <M, R>> func2);

        IEnumerable<T> ToList();
    }

    public class EnumerableMonad<T> : IMonad<IEnumerable, T>                   
    {
        private readonly IEnumerable<T> _enumerable;

        public EnumerableMonad(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IMonad<IEnumerable, R> Bind<R>(Func<T, IMonad<IEnumerable, R>> func2)
        {
            return _enumerable.SelectMany(t => func2(t).ToList()).ToMonad();
        }

        public IEnumerable<T> ToList()
        {
            return _enumerable;
        }
    }
}
