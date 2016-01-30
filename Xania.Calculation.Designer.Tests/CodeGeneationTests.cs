using FluentAssertions;
using NUnit.Framework;
using Xania.Calculation.Designer.Code;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Tests
{
    public class CodeGeneationTests
    {
        private CalculationCodeGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new CalculationCodeGenerator();
        }

        [TestCase(LeafType.Amount, "2 * input", "Leaf (fun input -> 2 * input)")]
        [TestCase(LeafType.Amount, "totalAmount", "Leaf totalAmount")]
        [TestCase(LeafType.Amount, "_fun", "Leaf _fun")]
        [TestCase(LeafType.Amount, "_fun2", "Leaf _fun2")]
        public void LeafTest(LeafType leafType, string fun, string expected)
        {
            // arrange
            var leaf = new LeafComponent
            {
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
                Name = "getResponse",
                Arguments =
                {
                    new Branch {Name = "b1", Tree = new LeafComponent {Fun = "totalAmount"}},
                    new Branch {Name = "b2", Tree = new LeafComponent {Fun = "score"}},
                }
            };

            // act
            var code = _generator.GenerateCode(node, null);

            // assert
            code.Should().Be("Node [ (\"b1\", Leaf totalAmount) ; (\"b2\", Leaf score) ]");
        }

        [Test]
        public void ConcatComponentTest()
        {
            var cancat = new ConcatComponent
            {
                new NodeComponent
                {
                    new Branch {Name = "b1", Tree = new LeafComponent {Fun = "totalAmount"}},
                },
                new NodeComponent
                {
                    new Branch {Name = "b2", Tree = new LeafComponent {Fun = "totalAmount"}},
                }
            };
            var code = _generator.GenerateCode(cancat, null);

            code.Should().Be("concat [ Node [ (\"b1\", Leaf totalAmount) ] ; Node [ (\"b2\", Leaf totalAmount) ] ]");
        }

        [TestCase(false, "personAge", "(\"b1\", mapTree personAge subtree )")]
        [TestCase(true, "personGrades", "(\"b1\", mapTrees personGrades subtree )")]
        public void SubTreeTest(bool many, string path, string expected)
        {
            // arrange
            var subTree = new TreeRefComponent { Name = "subtree" };
            var branch = new Branch { Name = "b1", Many = many, Path = path, Tree = subTree };

            // act
            var code = _generator.GenerateCode(branch, null);

            // assert
            code.Should().Be(expected);
        }
    }
}
