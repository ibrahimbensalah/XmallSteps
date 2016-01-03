using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class QueryableFunctionTests
    {
        private Organisation _organisation;

        [SetUp]
        public void Setup()
        {
            _organisation = new Organisation
            {
                Persons =
                {
                    new Person {Age = 60},
                    new Person {Age = 50},
                    new Person {Age = 55}
                }
            };
        }

        [Test]
        public void QueyableTest()
        {
            var q = 
                from o in Function.Query<Organisation>()
                from p in o.Persons
                select p.Age;

            q.Should().BeAssignableTo<IFunction<IQueryable<Organisation>, IQueryable<int>>>();

            q.Execute(new[] {_organisation}.AsQueryable()).Should().BeEquivalentTo(60, 50, 55);
        }

        [Test]
        public void ComposeStepTest()
        {
            // arrange
            var random = new Random();
            var composeStep = Function.Query<Organisation>()
                .Select(c => c.Persons)
                .Select(p => p.Age);

            // act
            var result = composeStep.Execute(_organisation).ToArray();

            // assert
            _organisation.Persons.Select(e => e.Age).ShouldBeEquivalentTo(result);
        }
    }
}
