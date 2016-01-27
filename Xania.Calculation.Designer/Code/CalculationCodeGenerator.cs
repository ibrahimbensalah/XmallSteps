using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Code
{
    public class CalculationCodeGenerator
    {
        public string GenerateCode(LeafComponent leaf)
        {
            return leaf.ToString();
        }

        public string GenerateCode(NodeComponent node)
        {
            return "Node ([ " + string.Join(" ; ",
                node.Arguments.Select(GenerateCode)) + " ])";
            // return string.Format("Node ([ Branch (\"branch\", {0}) ])");
        }

        public string GenerateCode(TreeArgument b)
        {
            if (string.IsNullOrEmpty(b.Path))
                return string.Format("Branch (\"{0}\", {1})", b.Name, GenerateCode(b.Tree));;

            return string.Format("Branch (\"{0}\", {1} {2} {3} )", b.Name, 
                b.Many ? "mapTrees": "mapTree", b.Path, GenerateCode(b.Tree)); ;
        }

        private string GenerateCode(ITreeComponent tree)
        {
            if (tree is LeafComponent)
                return GenerateCode(tree as LeafComponent);
            if (tree is NodeComponent)
                return GenerateCode(tree as NodeComponent);
            if (tree is TreeRefComponent)
                return GenerateCode(tree as TreeRefComponent);

            throw new NotSupportedException("type is not supported " + tree.GetType());
        }

        private string GenerateCode(TreeRefComponent treeRefComponent)
        {
            return treeRefComponent.Name;
        }
    }
}