using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Code
{
    public class CalculationCodeGenerator
    {
        public string GenerateCode(LeafComponent leaf)
        {
            if (leaf.IsIdentityCall)
                return "Id";
            if (leaf.IsFunctionCall)
                return string.Format("Leaf {0}", leaf.Fun);

            return string.Format("Leaf (fun input -> {0})", leaf.Fun);
        }

        public string GenerateCode(NodeComponent node)
        {
            return "let "+ node.Name + " input = Node ([ " + string.Join(" ; ",
                node.Arguments.Select(GenerateCode)) + " ])";
            // return string.Format("Node ([ Branch (\"branch\", {0}) ])");
        }

        public string GenerateCode(TreeArgument b)
        {
            if (string.IsNullOrEmpty(b.Path))
                return string.Format("(\"{0}\", {1})", b.Name, GenerateCode(b.Tree));;

            return string.Format("(\"{0}\", {1} {2} {3} )", b.Name, 
                b.Many ? "mapTrees": "mapTree", b.Path, GenerateCode(b.Tree)); ;
        }

        public string GenerateCode(ITreeComponent tree)
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