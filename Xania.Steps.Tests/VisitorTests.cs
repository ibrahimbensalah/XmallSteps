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
            var visitor = Substitute.For<IStepVisitor<Organisation>>();
            var org = new Organisation();
            // act
            root.Execute(org, visitor);
            // assert
            visitor.Received().Visit(org);
        }

        [Test]
        public void EachVisitorTest()
        {
            // arrange
            var root = Step.Root<Organisation>().Select(o => o.Persons).Each();
            var visitor = Substitute.For<IStepVisitor<Person>>();
            var org = new Organisation();
            // act
            root.Execute(org, visitor);
            // assert
            visitor.ReceivedCalls().Should().HaveCount(0);
        }
    }

    public class StepVisitor : IStep<Organisation>
    {
        public void Execute(Organisation model)
        {
        }
    }
}