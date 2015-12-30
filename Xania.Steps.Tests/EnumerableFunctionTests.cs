using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class EnumerableFunctionTests
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
            var agesStep = Function.Id<Organisation>()
                .Select(o => o.Persons)
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 1, 2 });
        }


        [Test]
        public void LinqTest()
        {
            var q = from f in Function.Id<Organisation>()
                    from p in f.Persons
                    select new { f, p };

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
            result.ShouldBeEquivalentTo(new[] { 60, 50, 55 });
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

        [Test]
        public void FunctionWhereTest()
        {
            var isAdult = Function.Create((Person p) => p.Age >= 18);
            var getAdults =
                from p in Function.Each<Person>()
                where isAdult
                where p.Age < 30
                select p;

            getAdults.Execute(new Person() { Age = 1 }, new Person() { Age = 21 }, new Person() { Age = 41 }).Should().HaveCount(1);
        }
    }
}
