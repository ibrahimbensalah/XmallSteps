using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void ComposeStepTest()
        {
            // arrange
            var random = new Random();
            var composeStep = Function.Id<Organisation>()
                .Select(c => c.Persons)
                .Select(new AssignFunction<Person, int>(p => p.Age, random.Next()))
                .Select(p => p.Age);

            // act
            var result = composeStep.Execute(_organisation).ToArray();

            // assert
            _organisation.Persons.Select(e => e.Age).ShouldBeEquivalentTo(result);
        }


        [Test]
        public void ForEachStepTest()
        {
            // arrange
            var person1 = new Person();
            var person2 = new Person();

            // act

            var func = Function.Each<Person>()
                .Select(new AssignFunction<Person, int>(p => p.Age, 132));

            foreach(var person in func.Execute(person1, person2))
                person.Age.Should().Be(132);
        }

        [Test]
        public void OrganisationAgeTest()
        {
            // arrange
            var organisation = new Organisation
            {
                Persons = { new Person() { Age = 1 }, new Person() { Age = 2 } }
            };

            // act
            var agesStep = Function.Id<Organisation>()
                .Select(o => o.Persons)
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 1, 2 });
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
                .Select(o => o.Persons)
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
                .Select(o => o.Persons)
                .Select(p => p.Age);

            selectStep.Execute(_organisation).ShouldBeEquivalentTo(new[] { 60, 50, 55 });
        }

        [Test]
        public void InvokeTest()
        {
            var selectStep = Function.Id<Organisation>()
                .Invoke(o => o.Init())
                .Select(o => o.Persons)
                .Select(p => p.Age);

            selectStep.Execute(_organisation).Should().BeEquivalentTo(60, 50, 55);
        }

        [Test]
        public void LinqTest()
        {
            var q = from f in Function.Id<Organisation>()
                from p in f.Persons
                select new{f, p};

            var r = q.Execute(_organisation).First();
            r.f.Should().NotBeNull();
        }

        [Test]
        public void BranchTest()
        {
            var selectStep = from o in Function.Id<Organisation>()
                from p in o.Persons
                select p.Age;

            var result = selectStep.Execute(_organisation);
            result.ShouldBeEquivalentTo(new[] {60, 50, 55});
        }

        [Test]
        public void ShouldBeAbleToNestMultipleFromConstructs()
        {
            var q = from c in Function.Id<Contract>()
                from o in c.Organisations
                from p in o.Persons
                from g in p.Grades
                select g;
        }

        [Test]
        public void JoinTest()
        {
            var personFunc = Function.Id<Organisation>().Select(e => e.Persons);
            var ageFunc = Function.Id<Organisation>().Select(e => e.Persons).Select(e => e.Age);
            var joinFunctor =
                from person in personFunc
                join age in ageFunc on person.Age equals age
                select person.Age + age;

            var result = joinFunctor.Execute(_organisation);
            result.ShouldBeEquivalentTo(new[] { 120, 100, 110 });
        }
    }
}