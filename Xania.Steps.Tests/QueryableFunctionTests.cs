using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class QueryableFunctionTests
    {
        [Test]
        public void QueyableTest()
        {
            var organisations = new[] {new Organisation()}.AsQueryable();
            var q = 
                from o in Function.Id<IQueryable<Organisation>>()
                from p in o.Persons
                select p;

            q.Should().BeOfType<IFunction<IQueryable<Organisation>, IQueryable<Person>>>();
        }
    }
}
