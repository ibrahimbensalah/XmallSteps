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

        [TestCase(LeafType.Amount, "2 * value", "Amount (fun value -> 2 * value)")]
        [TestCase(LeafType.Amount, "totalAmount", "Amount totalAmount")]
        [TestCase(LeafType.Amount, "_fun", "Amount _fun")]
        [TestCase(LeafType.Amount, "_fun2", "Amount _fun2")]
        public void LeafTest(LeafType leafType, string fun, string expected)
        {
            // arrange
            var leaf = new LeafComponent
            {
                Type = leafType,
                Fun = fun
            };

            // act
            var code = _generator.GenerateCode(leaf);

            // assert
            code.Should().Be(expected);
        }

        [Test]
        public void NodeBranchesTest()
        {
            var node = new NodeComponent
            {
                Branches =
                {
                    new BranchComponent {Name = "b1", Tree = new LeafComponent {Fun = "totalAmount"}},
                    new BranchComponent {Name = "b2", Tree = new LeafComponent {Fun = "score"}},
                }
            };

            // act
            var code = _generator.GenerateCode(node);

            // assert
            code.Should().Be("Node ([ Branch (\"b1\", Amount totalAmount) ; Branch (\"b2\", Amount score) ])");
        }

        [TestCase(false, "personAge", "Branch (\"b1\", mapTree personAge subtree )")]
        [TestCase(true, "personGrades", "Branch (\"b1\", mapTrees personGrades subtree )")]
        public void SubTreeTest(bool many, string path, string expected)
        {
            // arrange
            var subTree = new TreeRefComponent { Name = "subtree" };
            var branch = new BranchComponent {Name = "b1", Many = many, Path = path, Tree = subTree};

            // act
            var code = _generator.GenerateCode(branch);

            // assert
            code.Should().Be(expected);
        }
    }
}
