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
    public class FunctorTests
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
            var fun = new Functor<int, int>(a => a * 2);
            fun.Execute(1).Should().Be(2);
        }

        [Test]
        public void ComposeFunctionTest()
        {
            var fun = new Functor<int, int>(a => a * 2).Compose(new Functor<int, int>(a => a * 2));
            fun.Execute(1).Should().Be(4);
        }

        [Test]
        public void EachMonadTest()
        {
            var fun = new Functor<Organisation, EachMonad<Person>>(o => o.Persons.Each());
            fun.Execute(new Organisation()).Should().BeEmpty();
        }

        [Test]
        public void MonadCompositionTest()
        {
            var organisation = new Organisation { Persons = { new Person { Age = 10 }, new Person { Age = 11 } } };
            var personAgeFunc = new Functor<Person, int>(p => p.Age);
            var eachPersonFunc = new Functor<Organisation, EachMonad<Person>>(o => o.Persons.Each());
            var fun = eachPersonFunc.Select(e => e.Age);

            fun.Execute(organisation).Should().BeEquivalentTo(new[] { 10, 11 });
        }


        [Test]
        public void ComposeStepTest()
        {
            // arrange
            var random = new Random();
            var composeStep = Functor.Id<Organisation>()
                .Select(c => c.Persons)
                .ForEach(new AssignFunctor<Person, int>(p => p.Age, random.Next()))
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
            var func = Functor.Each<Person>().ForEach(new AssignFunctor<Person, int>(p => p.Age, 132));

            EachExtensions.Execute(func, person1, person2);
                // .ForEach(new AssignFunctor<Person, int>(p => p.Age, 132)); // .Execute(person1, person2);

            // assert
            person1.Age.Should().Be(132);
            person2.Age.Should().Be(132);
        }
    }
}