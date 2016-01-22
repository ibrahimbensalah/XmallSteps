using FluentAssertions;
using NUnit.Framework;

namespace Xania.Calculation.Designer.Tests
{
    public class CodeGeneationTests
    {
        private CalculationCodeGenerator _generator;

        [SetUp]
        public void Setup()
        {
            _generator = new CalculationCodeGenerator();
        }

        [Test]
        public void LeafTest()
        {
            // arrange
            var leaf = new LeafComponent
            {
                Type = LeafType.Amount,
                Expression = "2 * input + 2"
            };

            // act
            var code = _generator.GenerateCode(leaf);

            // assert
            code.Should().Be("Amount (fun input -> 2 * input + 2)");
        }

        [Test]
        public void NodeTest()
        {
            var node = new NodeComponent
            {
                Branches =
                {
                    new BranchComponent {IsList = false, Name = "branch1", Tree = new LeafComponent {Expression = "1m"}},
                    new BranchComponent {IsList = true, Path = "path1", Name = "branch2", Tree = new LeafComponent {Expression = "3m"}}
                }
            };

            // act
            var code = _generator.GenerateCode(node);

            // assert
            code.Should().Be("Node ([ Branch (\"branch1\", Amount (fun input -> 1m)) ; Branch (\"branch2\", map (path1) (Amount (fun input -> 3m))) ])");
        }
    }
}
