using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class FunctionTests
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
        public void TrivialFunctionTest()
        {
            var fun = new Function<int, int>("f", a => a * 2);
            fun.Execute(1).Should().Be(2);
        }

        [Test]
        public void ComposeFunctionTest()
        {
            var fun = new Function<int, int>("f", a => a * 2).Compose(new Function<int, int>("g", a => a * 2));
            fun.Execute(1).Should().Be(4);
        }

        [Test]
        public void EachMonadTest()
        {
            var fun = new Function<Organisation, IEnumerable<Person>>("f", o => o.Persons);
            fun.Execute(new Organisation()).Should().BeEmpty();
        }

        [Test]
        public void MonadCompositionTest()
        {
            var organisation = new Organisation { Persons = { new Person { Age = 10 }, new Person { Age = 11 } } };
            var eachPersonFunc = new Function<Organisation, IEnumerable<Person>>("g", o => o.Persons);
            var fun = eachPersonFunc.Select(e => e.Age);

            fun.Execute(organisation).Should().BeEquivalentTo(new[] { 10, 11 });
        }

        [Test]
        public void OrganisationRootTest()
        {
            // arrange
            var organisation = new Organisation
            {
                Persons =
                {
                    new Person { Age = 1 }, 
                    new Person { Age = 2 }
                }
            };

            // act
            var agesStep = Function.Id<Organisation>()
                .Member(o => o.Persons)
                .ForEach(p => { p.Age = 123; })
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 123, 123 });
        }

        [Test]
        public void BranchMergeTest()
        {
            var selectStep = Function.Id<Organisation>()
                .Invoke(o => o.Init())
                .Member(o => o.Persons)
                .Select(p => p.Age);

            selectStep.Execute(_organisation).ShouldBeEquivalentTo(new[] { 60, 50, 55 });
        }

        [Test]
        public void InvokeTest()
        {
            var selectStep = Function.Id<Organisation>()
                .Invoke(o => o.Init())
                .Member(o => o.Persons)
                .Select(p => p.Age);

            selectStep.Execute(_organisation).Should().BeEquivalentTo(60, 50, 55);
        }
    }
}