using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Xania.Steps.Tests
{
    public class VisitorTests
    {
        [Test]
        public void RootVisitorTest()
        {
            // arrange
            var root = Step.Root<Organisation>();
            var org = new Organisation();
            // act
            var visitor = Substitute.For<IStepVisitor<Organisation>>();
            root.Execute(org, visitor);
            // assert
            visitor.Received().Visit(org);
        }

        [Test]
        public void EachVisitorTest()
        {
            // arrange
            var root = Step.Root<Organisation>().Select(o => o.Persons).Each();
            var org = new Organisation
            {
                Persons =
                {
                    new Person(),
                    new Person()
                }
            };
            var visitor = Substitute.For<IStepVisitor<Person>>();
            // act
            root.Execute(org, visitor);
            // assert
            visitor.ReceivedCalls().Should().HaveCount(org.Persons.Count);
        }
    }

    public class StepVisitor : IStep<Organisation>
    {
        public void Execute(Organisation model)
        {
        }
    }
}