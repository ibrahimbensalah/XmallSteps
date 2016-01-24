using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xania.Calculation.Designer
{
    public class CalculationCodeGenerator
    {
        public string GenerateCode(LeafComponent leaf)
        {
            if (Regex.IsMatch(leaf.Fun, "^[_a-z][_a-z0-9]+$", RegexOptions.IgnoreCase))
                return string.Format("{0} {1}", leaf.Type, leaf.Fun);
            return string.Format("{0} (fun value -> {1})", leaf.Type, leaf.Fun);
        }

        public string GenerateCode(NodeComponent node)
        {
            return "Node ([ " + string.Join(" ; ",
                node.Branches.Select(GenerateCode)) + " ])";
            // return string.Format("Node ([ Branch (\"branch\", {0}) ])");
        }

        public string GenerateCode(BranchComponent b)
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