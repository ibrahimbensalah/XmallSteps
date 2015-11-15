using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Xania.Steps.Tests
{
    public class StepTests
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
        public void LazyDataTest()
        {
            // arrange
            var lazy = new LazyValue<int>(() => 3456);

            // act & verify
            lazy.Get().Should().Be(3456);
        }

        [Test]
        public void SumTest()
        {
            // arrange
            var step = Step.Aggregate<int>(g => g.Sum());

            // act & verify
            step.Execute(1, 2, 3).Should().BeEquivalentTo(6);
        }

        [Test]
        public void AssignStepTest()
        {
            // arrange
            var step = Step.Assign((Person e) => e.Age, 33);
            var person = new Person();

            // act
            step.Execute(person);

            // verify
            person.Age.Should().Be(33);
        }

        [Test]
        public void ComposeStepTest()
        {
            // arrange
            var random = new Random();
            var composeStep = Step.Select((Organisation c) => c.Persons).Each()
                .Assign(p => p.Age, random.Next())
                .Select(p => p.Age);

            // act
            var result = composeStep.Execute(_organisation).ToArray();

            // assert
            _organisation.Persons.Select(e => e.Age).ShouldBeEquivalentTo(result);
        }

        [Test]
        public void SequenceStepTest()
        {
            var sequence = Step.Sequence(new AssignAgeStep(1), new AssignAgeStep(2));

            var person = new Person();
            sequence.Execute(person);

            person.Age.Should().Be(2);
        }

        [Test]
        public void ForEachStepTest()
        {
            // arrange
            var person1 = new Person();
            var person2 = new Person();

            // act
            Step.Each<Person>().Compose(new PersonAgeStep()).Execute(new[] { person1, person2 });

            // assert
            person1.Age.Should().Be(123);
            person2.Age.Should().Be(123);
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
            var agesStep = Step.Select((Organisation o) => o.Persons).Each()
                .Compose(new PersonAgeStep());

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
            var agesStep = Step.Select((Organisation o) => o.Persons).Each()
                .Assign(p => p.Age, 123)
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 123, 123 });
        }

        [Test]
        public void StepRootTest()
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
            var agesStep = Step.Root<Organisation>()
                .Select(o => o.Persons).Each()
                .Assign(p => p.Age, 123)
                .Select(p => p.Age);

            // assert
            agesStep.Execute(organisation).ShouldBeEquivalentTo(new[] { 123, 123 });
        }

        [Test]
        public void InvokeTest()
        {
            var selectStep = Step.Root<Organisation>()
                .Invoke(o => o.Init())
                .Select(o => o.Persons).Each()
                .Select(p => p.Age);

            selectStep.Execute(_organisation).Should().BeEquivalentTo(60, 50, 55);
        }

        [Test]
        public void BranchMergeTest()
        {
            var selectStep = Step.Root<Organisation>()
                .Compose(Step.Root<Organisation>().Invoke(o => o.Init()).Select(o => o.Persons).Each().Select(p => p.Age));

            selectStep.Execute(_organisation).ShouldBeEquivalentTo(new[] { 60, 50, 55 });
        }

        [Test]
        public void BranchTest()
        {
            var selectStep = Step.Root<Organisation>()
                .Invoke(o => o.Init())
                .Branch(Step.Root<Organisation>().Each(o => o.Persons).Select(p => p.Age))
                .Each(o => o.Persons)
                .Select(p => p.Age);

            var result = selectStep.Execute(_organisation);
            result.ShouldBeEquivalentTo(new[] { 60, 50, 55 });
        }

        [Test]
        public void BranchSelectorTest()
        {
            var branchStep = Step.Root<Organisation>()
                .Branch(o => o.Persons, r => r.Each().Assign(p => p.Age, 123))
                .Branch(r => r.Invoke(o => o.Init()));

            // act
            branchStep.Execute(_organisation);

            // assert
            _organisation.Persons.Select(p => p.Age).Should().BeEquivalentTo(new[] { 123, 123, 123 });
            _organisation.TotalAge.Should().Be(123 * 3);
        }

        [Test]
        public void WhenStepTest()
        {
            var branchStep = Step.Root<Person>()
                .When(p => p.Age > 18, r => r.Assign(p => p.Age, 18))
                .When(p => p.Age < 14, r => r.Assign(p => p.Age, 14));

            var person = new Person {Age = 11};

            // act
            branchStep.Execute(person);

            // assert
            person.Age.Should().Be(14);
        }
    }
}
