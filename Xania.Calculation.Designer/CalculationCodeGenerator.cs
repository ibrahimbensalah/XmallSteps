using System;
using System.Linq;

namespace Xania.Calculation.Designer
{
    public class CalculationCodeGenerator
    {
        public string GenerateCode(LeafComponent leaf)
        {
            return string.Format("{0} (fun input -> {1})", leaf.Type, leaf.Expression);
        }

        public string GenerateCode(NodeComponent node)
        {
            return "Node ([ " + string.Join(" ; ", 
                node.Branches.Select(Format)) + " ])";
            // return string.Format("Node ([ Branch (\"branch\", {0}) ])");
        }

        private string Format(BranchComponent b)
        {
            if (string.IsNullOrEmpty(b.Path))
                return string.Format("Branch (\"{0}\", {1})", b.Name, GenerateCode(b.Tree));;

            if (b.IsList)
                return string.Format("BranchMany (\"{0}\", map {1} {2})", b.Name, b.Path, GenerateCode(b.Tree)); ;
        }

        private string GenerateCode(ITreeComponent tree)
        {
            if (tree is LeafComponent)
                return GenerateCode(tree as LeafComponent);
            if (tree is NodeComponent)
                return GenerateCode(tree as NodeComponent);

            throw new NotSupportedException("type is not supported " + tree.GetType());
        }
    }
}