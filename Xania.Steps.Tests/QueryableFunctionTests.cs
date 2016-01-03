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
            var composeStep = Function.Query<Organisation>()
                .SelectMany(c => c.Persons)
                .Select(p => p.Age);
            var input = new[] {_organisation}.AsQueryable();

            // act
            var result = composeStep.Execute(input).ToArray();

            // assert
            _organisation.Persons.Select(e => e.Age).ShouldBeEquivalentTo(result);
        }

        [Test]
        public void EachStepTest()
        {
            // arrange
            var person1 = new Person();
            var person2 = new Person();

            // act

            var func = Function.Query<Person>()
                .Select(new AssignFunction<Person, int>(p => p.Age, 132));

            foreach (var person in func.Execute(person1, person2))
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
            var agesStep = Function.Query<Organisation>()
                .SelectMany(o => o.Persons)
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 1, 2 });
        }

        [Test]
        public void LinqTest()
        {
            var q = from f in Function.Query<Organisation>()
                    from p in f.Persons
                    select new { f, p };

            var r = q.Execute(_organisation).First();
            r.f.Should().NotBeNull();
        }

        [Test]
        public void ShouldBeAbleToNestMultipleFromConstructs()
        {
            var q = from c in Function.Query<Contract>()
                    from o in c.Organisations
                    from p in o.Persons
                    from g in p.Grades
                    select g;
        }

        [Test]
        public void JoinTest()
        {
            var personFunc = Function.Query<Organisation>().SelectMany(e => e.Persons);
            var ageFunc = Function.Query<Organisation>().SelectMany(e => e.Persons).Select(e => e.Age);
            var joinFunctor =
                from person in personFunc
                join age in ageFunc on person.Age equals age
                select person.Age + age;

            var result = joinFunctor.Execute(_organisation);
            result.ShouldBeEquivalentTo(new[] { 120, 100, 110 });
        }
    }
}
